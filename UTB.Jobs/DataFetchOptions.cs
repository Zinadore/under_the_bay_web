using System.Collections.Generic;
using CronScheduler.Extensions.Scheduler;
using UTB.Contracts.DTO;

namespace UTB.API.Jobs
{
    public class DataFetchOptions: SchedulerOptions
    {
        public bool AddStations { get; set; }

        // ReSharper disable once InconsistentNaming
        public string RequestURI { get; set; }
        
        public string TimeZone { get; set; }
        public List<StationDTO> Stations { get; set; }
    }
}