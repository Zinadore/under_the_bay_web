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

            if (record.SampleDepth_m != null)
                sample.SampleDepth = record.SampleDepth_m ?? Math.Clamp(record.SampleDepth_m.Value, 0.0f, 200.0f);
            if (record.Temp_C != null)
                sample.WaterTemperature = record.Temp_C ?? Math.Clamp(record.Temp_C.Value, 0.0f, 100.0f);
            if (record.DO_mgL != null)
                sample.DissolvedOxygen = record.DO_mgL ?? Math.Clamp(record.DO_mgL.Value, 0.0f, 21.0f);
            if (record.DO_sat != null)
                sample.DissolvedOxygenSaturation = record.DO_sat ?? Math.Clamp(record.DO_sat.Value, 0.0f, 1.0f);
            if (record.Salinity_ppt != null)
                sample.Salinity = record.Salinity_ppt ?? Math.Clamp(record.Salinity_ppt.Value, 0.0f, 32.0f);
            if (record.pH != null)
                sample.pH = record.pH ?? Math.Clamp(record.pH.Value, 0.0f, 14.0f);
            if (record.Turbidity_NTU != null)
                sample.Turbidity = record.Turbidity_NTU ?? Math.Clamp(record.Turbidity_NTU.Value, 0.0f, 100.0f);
            if (record.ChlA_ugL != null)
                sample.Chlorophyll = record.ChlA_ugL ?? Math.Clamp(record.ChlA_ugL.Value, 0.0f, 100.0f); 
            if (record.ChlA_ugL != null)
                sample.BlueGreenAlgae = record.ChlA_ugL ?? Math.Clamp(record.ChlA_ugL.Value, 0.0f, 1000.0f); 
            
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

        public string Name { get; } = nameof(DataFetchJob);
    }
}