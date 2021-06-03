using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CronScheduler.Extensions.Scheduler;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Text;
using Under_the_Bay.Data;
using Under_the_Bay.Data.Models;

namespace Under_the_Bay.API.Jobs
{
    public class DataFetchJob: IScheduledJob
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<DataFetchJob> _logger;
        private readonly DataFetchOptions _options;
        
        private readonly LocalDatePattern _datePattern;
        private readonly LocalTimePattern _timePattern;
        private readonly DateTimeZone _timeZone;

        public DataFetchJob(IServiceProvider provider, IOptionsMonitor<DataFetchOptions> options,
            ILogger<DataFetchJob> logger)
        {
            _provider = provider;
            _logger = logger;
            _options = options.Get(Name);
            
            _datePattern = LocalDatePattern.CreateWithCurrentCulture("MM/dd/yyyy");
            _timePattern = LocalTimePattern.CreateWithCurrentCulture("hh:mm:ss");
            _timeZone = DateTimeZoneProviders.Tzdb[_options.TimeZone];
        }
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, "Running data fetch");

            using var scope = _provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UtbContext>();
            
            if (_options.AddStations)
                await AddStations(context);

            var stations = await context.Stations.ToListAsync(cancellationToken);

            foreach (var station in stations)
            {
                DateTimeOffset now = DateTimeOffset.Now;
                
                var startString = station.LastUpdate.InZone(_timeZone)
                    .ToString("yyyy/MM/dd", CultureInfo.CurrentCulture);
                var endString = now.ToString("yyyy/MM/dd");

                string url = string.Format(_options.RequestURI, station.ThreeLetterId, startString, endString);

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

                        await context.Samples.AddAsync(sample, cancellationToken: cancellationToken);
                    }

                    station.LastUpdate = newestTime;
                    if (await context.SaveChangesAsync(cancellationToken) > 0)
                        _logger.Log(LogLevel.Information, "Done");
                    else
                        _logger.Log(LogLevel.Information, $"No records added from {csvFile.FullName}");
                }
                File.Delete(csvFile.FullName);
            }
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
        private Sample MapSample(SampleRecord record)
        {
            Sample sample = new Sample();

            sample.SampleDepth = MapProperty(record.SampleDepth_m, 0, 200);
            sample.WaterTemperature = MapProperty(record.Temp_C, 0, 100);
            sample.DissolvedOxygen = MapProperty(record.DO_mgL, 0, 21);
            sample.DissolvedOxygenSaturation = MapProperty(record.DO_sat, 0.0f, 1);
            sample.Salinity = MapProperty(record.Salinity_ppt, 0, 32);
            record.pH = MapProperty(record.pH, 0, 14);
            sample.Turbidity = MapProperty(record.Turbidity_NTU, 0, 100);
            sample.Chlorophyll = MapProperty(record.ChlA_ugL, 0, 100);
            sample.BlueGreenAlgae = MapProperty(record.BGA_RFU, 0, 1000);  
            
            return sample;
        }

        private FileInfo DownloadFile(string url, string stationThreeLetterId)
        {
            using var client = new HttpClient();
            var fileInfo = new FileInfo($"{stationThreeLetterId}.csv");
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
            foreach (var station in _options.Stations)
            {
                var exists = await context.Stations.AnyAsync(s => s.ThreeLetterId == station.ThreeLetterId);

                if (exists)
                    continue;

                await context.Stations.AddAsync(station);
            }

            await context.SaveChangesAsync();
        }
        
#nullable enable
        private float MapProperty(float? source, float min, float max)
        {
            return source != null ? Math.Clamp(source.Value, min, max) : min;
        }
#nullable disable
        
        public string Name { get; } = nameof(DataFetchJob);
    }
}