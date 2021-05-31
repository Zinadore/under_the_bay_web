using System;
using Microsoft.AspNetCore.Mvc;


namespace Under_the_Bay.API.V1.Contracts.Requests
{
    public class StationRequest
    {
        [FromRoute(Name = "id")] 
        public Guid Id { get; set; }
        [FromQuery(Name = "include_measurements")]
        public bool IncludeMeasurements { get; set; }
        [FromQuery(Name = "start_date")]
        public DateTimeOffset? StartDate { get; set; }
        [FromQuery(Name = "end_date")]
        public DateTimeOffset? EndDate { get; set; }
    }
}