using System;
using System.Collections.Generic;
using NodaTime;

namespace UTB.API.V1.Contracts.Responses
{
    public class StationResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// In ISO-8061 format
        /// </summary>
        public DateTimeOffset LastUpdate { get; set; }

        public List<SampleResponse> Samples { get; set; }
    }
}