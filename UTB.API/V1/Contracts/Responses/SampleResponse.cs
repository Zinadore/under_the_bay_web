using System;
using NodaTime;

namespace UTB.API.V1.Contracts.Responses
{
    public class SampleResponse
    {
        /// <summary>
        /// In ISO-8061 format
        /// </summary>
        public DateTimeOffset SampleDate { get; set; }
        public float WaterTemperature { get; set; }
        public float DissolvedOxygen { get; set; }
        // public float DissolvedOxygenSaturation { get; set; }
        public float Salinity { get; set; }
        // ReSharper disable once InconsistentNaming
        public float PH { get; set; }
        public float Turbidity { get; set; }
        public float Chlorophyll { get; set; }
        // public float BlueGreenAlgae { get; set; }
    }
}