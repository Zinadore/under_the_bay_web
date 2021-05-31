using System;

namespace Under_the_Bay.API.V1.Contracts.Responses
{
    public class StationResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        // public DateTimeOffset LastUpdate { get; set; }
    }
}