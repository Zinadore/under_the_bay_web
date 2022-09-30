using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CronScheduler.Extensions.Scheduler;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Text;
using UTB.Contracts.DTO;
using UTB.Data;
using UTB.Data.Models;
using UTB.Data.Services;

namespace UTB.API.Jobs
{
    public class DataFetchJob: IScheduledJob
    {
        private readonly IStationsService stationsService;
        private readonly ISamplesService recordsService;
        private readonly UtbContext context;
        private readonly ILogger<DataFetchJob> _logger;
        private readonly DataFetchOptions _options;
        
        private readonly LocalDatePattern _datePattern;
        private readonly LocalTimePattern _timePattern;
        private readonly DateTimeZone _timeZone;

        public DataFetchJob(
            IStationsService stationsService,
            ISamplesService recordsService,
            UtbContext context,
            IOptionsMonitor<DataFetchOptions> options,
            ILogger<DataFetchJob> logger)
        {

            this.stationsService = stationsService;
            this.recordsService = recordsService;
            this.context = context;
            _logger = logger;
            _options = options.Get(Name);
            
            _datePattern = LocalDatePattern.CreateWithCurrentCulture("MM/dd/yyyy");
            _timePattern = LocalTimePattern.CreateWithCurrentCulture("H:mm:ss");
            _timeZone = DateTimeZoneProviders.Tzdb[_options.TimeZone];
        }
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var databaseReady = await context.CheckIfDatabaseReady();
            if (!databaseReady){
                _logger.LogInformation("Database is not ready yet, skipping this fetch");
                return;
            }


            _logger.Log(LogLevel.Information, "Running data fetch");

            if (_options.AddStations)
                await AddStations(cancellationToken);

            var stations = await stationsService.GetAll();

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
                    var records = csv.GetRecords<SampleFromCSV>();

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

                        await recordsService.AddAsync(record, instant.Value, station.Id);
                    }

                    station.LastUpdate = newestTime;
                    var numChanged = await stationsService.SaveChanges();
                    if (numChanged != 0)
                        _logger.Log(LogLevel.Information, "Done");
                    else
                        _logger.Log(LogLevel.Information, $"No records added from {csvFile.FullName}");
                }
                File.Delete(csvFile.FullName);
            }
        }
        private Instant? GetInstantFromRecord(SampleFromCSV record)
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

        private async Task AddStations(CancellationToken cancellationToken)
        {
            var existingStations = await stationsService.GetAll();

            foreach (var dto in _options.Stations)
            {
                var exists =  existingStations.Any(s => s.ThreeLetterId == dto.ThreeLetterId);

                if (exists)
                    continue;

                await stationsService.AddAsync(dto, cancellationToken);
            }

            await stationsService.SaveChanges();
        }
        
        public string Name { get; } = nameof(DataFetchJob);
    }
}