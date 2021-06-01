using System.Collections.Generic;
using CronScheduler.Extensions.Scheduler;
using Under_the_Bay.Data.Models;

namespace Under_the_Bay.API.Jobs
{
    public class DataFetchOptions: SchedulerOptions
    {
        public bool AddStations { get; set; }

        // ReSharper disable once InconsistentNaming
        public string RequestURI { get; set; }
        
        public string TimeZone { get; set; }
        public List<Station> Stations { get; set; }
    }
}