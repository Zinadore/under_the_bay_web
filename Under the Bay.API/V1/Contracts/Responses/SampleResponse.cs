using System;

namespace Under_the_Bay.API.V1.Contracts.Responses
{
    public class SampleResponse
    {
        public DateTimeOffset SampleDate { get; set; }
        public float WaterTemperature { get; set; }
        public float DissolvedOxygen { get; set; }
        public float DissolvedOxygenSaturation { get; set; }
        public float Salinity { get; set; }
        // ReSharper disable once InconsistentNaming
        public float PH { get; set; }
        public float Turbidity { get; set; }
        public float Chlorophyll { get; set; }
        public float BlueGreenAlgae { get; set; }
    }
}