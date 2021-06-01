using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private IConfiguration Configuration { get; }
        
        public DataFetcherApplication(IConfiguration configuration, UtbContext context, IHostApplicationLifetime lifetime, ILogger<DataFetcherApplication> logger)
        {
            _context = context;
            _lifetime = lifetime;
            _logger = logger;
            Configuration = configuration;
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
        
        public void Run()
        {
            bool shouldAddStations = Configuration.GetValue<bool>("UTB:AddStations");

            if (shouldAddStations)
            {
                foreach (var station in Stations)
                {
                    var exists = _context.Stations.Any(s => s.ThreeLetterId == station.ThreeLetterId);

                    if (exists)
                        continue;

                    _context.Stations.Add(station);
                }

                _context.SaveChanges();
            }
            else
            {
                Stations = _context.Stations.ToList();
            }

            string urlFormat = Configuration.GetValue<string>("UTB:RequestURI");
            
            // The data we get have the time set to EST. We always use this timezone, just in case
            // we get located to somewhere else.
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
            foreach (var station in Stations)
            {
                DateTimeOffset now = DateTimeOffset.Now;
                var startString = station.LastUpdate.ToString("yyyy/MM/dd");
                var endString = now.ToString("yyyy/MM/dd");

                string url = string.Format(urlFormat, station.ThreeLetterId, startString, endString);

                FileInfo csvFile = DownloadFile(url, station.ThreeLetterId);
                
                _logger.Log(LogLevel.Information, $"Processing file: {csvFile.FullName}...");
                
                DateTimeOffset newestTime = station.LastUpdate;
                
                using (var reader = new StreamReader(csvFile.FullName))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<SampleRecord>();

                    foreach (var record in records)
                    {
                        var dt = DateTime.Parse($"{record.SampleDate} {record.SampleTime}", CultureInfo.InvariantCulture);
                        var dto = new DateTimeOffset(dt, tz.GetUtcOffset(dt));

                        if (dto <= station.LastUpdate)
                            continue;

                        if (dto >= newestTime)
                            newestTime = dto;
                        
                        Sample sample = new Sample();

                        sample.SampleDate = dto;
                        sample.SampleDepth = record.SampleDepth_m is > -200.0f ? record.SampleDepth_m.Value : -1.0f;
                        sample.WaterTemperature = record.Temp_C is > -200.0f ? record.Temp_C.Value : -1.0f;
                        sample.DissolvedOxygen = record.DO_mgL is > -200.0f ? record.DO_mgL.Value : -1.0f;
                        sample.DissolvedOxygenSaturation = record.DO_sat is > -200.0f ? record.DO_sat.Value : -1.0f;
                        sample.Salinity = record.Salinity_ppt is > -200.0f ? record.Salinity_ppt.Value : -1.0f;
                        sample.PH = record.pH is > -200.0f ? record.pH.Value : -1.0f;
                        sample.Turbidity = record.Turbidity_NTU is > -200.0f ? record.Turbidity_NTU.Value : -1.0f;
                        sample.Chlorophyll = record.ChlA_ugL is > -200.0f ? record.ChlA_ugL.Value : -1.0f;
                        sample.BlueGreenAlgae =  record.ChlA_ugL is > -200 ? record.ChlA_ugL.Value: -1.0f ;
                        sample.StationId = station.Id;

                        _context.Samples.Add(sample);
                    }

                    station.LastUpdate = newestTime;
                    if (_context.SaveChanges() > 0)
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