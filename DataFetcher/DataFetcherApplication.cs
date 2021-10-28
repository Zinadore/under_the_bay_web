using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;
using Under_the_Bay.Data;
using Under_the_Bay.Data.Models;

namespace DataFetcher
{
    public class DataFetcherApplication
    {
        private static List<Station> Stations = new List<Station>
        {
            new Station { ThreeLetterId = "OPC", Name = "Otter Point Creek", StationId = "XJG7035", Layer = "BS" },
            new Station { ThreeLetterId = "MSC", Name = "Masonville Cove Pier", StationId = "XIE4742", Layer = "BS" },
            new Station { ThreeLetterId = "MAB", Name = "Mallows Bay Buoy", StationId = "XDA8236", Layer = "BS" },
            new Station { ThreeLetterId = "LMN", Name = "Little Monie", StationId = "LMN0028", Layer = "BS" },
            new Station { ThreeLetterId = "AWS", Name = "Aquarium West", StationId = "XIE7135", Layer = "BS" },
            new Station { ThreeLetterId = "AES", Name = "Aquarium East", StationId = "XIE7136", Layer = "S" },
            new Station { ThreeLetterId = "AEB", Name = "Aquarium East Bottom", StationId = "XIE7136", Layer = "B" },
        };
        
        
        private readonly UtbContext _context;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<DataFetcherApplication> _logger;
        
        private readonly LocalDatePattern _datePattern;
        private readonly LocalTimePattern _timePattern;
        private readonly DateTimeZone _timeZone;
        
        
        private IConfiguration Configuration { get; }

        public DataFetcherApplication(IConfiguration configuration, UtbContext context,
            IHostApplicationLifetime lifetime, ILogger<DataFetcherApplication> logger)
        {
            _context = context;
            _lifetime = lifetime;
            _logger = logger;
            Configuration = configuration;
            
            _datePattern = LocalDatePattern.CreateWithCurrentCulture("MM/dd/yyyy");
            _timePattern = LocalTimePattern.CreateWithCurrentCulture("hh:mm:ss");
            _timeZone = DateTimeZoneProviders.Tzdb[Configuration.GetValue<string>("UTB:Timezone")];
        }

        private FileInfo DownloadFile(string url, string stationName)
        {
            using var client = new HttpClient();
            var fileInfo = new FileInfo($"{stationName}.csv");
            var response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            using var stream = response.Content.ReadAsStream();
            using var file = File.Create(fileInfo.FullName);
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(file);

            return fileInfo;
        }
        
        private async Task AddStations(UtbContext context)
        {
            foreach (var station in Stations)
            {
                var exists = await context.Stations.AnyAsync(s => s.ThreeLetterId == station.ThreeLetterId);

                if (exists)
                    continue;

                await context.Stations.AddAsync(station);
            }

            await context.SaveChangesAsync();
        }
        
        private Instant? GetInstantFromRecord(SampleRecord record)
        {
            ParseResult<LocalDate> dateResult = _datePattern.Parse(record.SampleDate);
            ParseResult<LocalTime> timeResult = _timePattern.Parse(record.SampleTime);

            if (!dateResult.Success || !timeResult.Success)
                return null;
            
            var day = dateResult.Value;
            var time = timeResult.Value;
            
            var dateTime = day.At(time).InZoneLeniently(_timeZone);
            return dateTime.ToInstant();
        }
        
#nullable enable
        private float MapProperty(float? source, float min, float max)
        {
            return source != null ? Math.Clamp(source.Value, min, max) : min;
        }
#nullable disable
        
        private Sample MapSample(SampleRecord record)
        {
            Sample sample = new Sample();

            sample.SampleDepth = MapProperty(record.SampleDepth_m, 0, 200);
            sample.WaterTemperature = MapProperty(record.Temp_C, 0, 100);
            sample.DissolvedOxygen = MapProperty(record.DO_mgL, 0, 21);
            sample.DissolvedOxygenSaturation = MapProperty(record.DO_sat, 0.0f, 1);
            sample.Salinity = MapProperty(record.Salinity_ppt, 0, 32);
            sample.pH = MapProperty(record.pH, 0, 14);
            sample.Turbidity = MapProperty(record.Turbidity_NTU, 0, 100);
            sample.Chlorophyll = MapProperty(record.ChlA_ugL, 0, 100);
            sample.BlueGreenAlgae = MapProperty(record.BGA_RFU, 0, 1000); 
            
            return sample;
        }
        
        public async Task RunAsync()
        {
            bool shouldAddStations = Configuration.GetValue<bool>("UTB:AddStations");

            if (shouldAddStations)
                await AddStations(_context);
            
            Stations = _context.Stations.Where(x => x.Name == "Little Monie").ToList();
            

            string urlFormat = Configuration.GetValue<string>("UTB:RequestURI");
            
            foreach (var station in Stations)
            {
                DateTimeOffset now = DateTimeOffset.Now;
                
                var startString = station.LastUpdate.InZone(_timeZone)
                    .ToString("yyyy/MM/dd", CultureInfo.CurrentCulture);
                var endString = now.ToString("yyyy/MM/dd");

                string url = string.Format(urlFormat, station.ThreeLetterId, startString, endString);

                FileInfo csvFile = DownloadFile(url, station.ThreeLetterId);

                _logger.Log(LogLevel.Information, $"Processing file: {csvFile.FullName}...");

                Instant newestTime = station.LastUpdate;
                
                using (var reader = new StreamReader(csvFile.FullName))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<SampleRecord>();

                    foreach (var record in records)
                    {
                        Instant? instant = GetInstantFromRecord(record);

                        if (!instant.HasValue)
                        {
                            _logger.LogError($"Failed to parse date {record.SampleDate} {record.SampleTime}");
                            continue;
                        }
                        
                        if (instant <= station.LastUpdate)
                            continue;

                        if (instant >= newestTime)
                            newestTime = instant.Value;
                        
                        Sample sample = MapSample(record);

                        sample.SampleDate = instant.Value;
                        sample.StationId = station.Id;

                        await _context.Samples.AddAsync(sample);
                    }

                    station.LastUpdate = newestTime;
                    if (await _context.SaveChangesAsync() > 0)
                        _logger.Log(LogLevel.Information, "Done");
                    else
                        _logger.Log(LogLevel.Information, $"No records added from {csvFile.FullName}");
                }
                File.Delete(csvFile.FullName);
            }
            
            
            _lifetime.StopApplication();
            return;
        }
    }
}