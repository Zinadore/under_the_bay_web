using NodaTime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTB.Contracts.DTO
{
    public class SampleDTO
    {
        public Instant SampleDate { get; set; }
        public float SampleDepth { get; set; }
        public float WaterTemperature { get; set; }
        public float DissolvedOxygen { get; set; }
        public float DissolvedOxygenSaturation { get; set; }
        public float Salinity { get; set; }
        // ReSharper disable once InconsistentNaming
        public float pH { get; set; }
        public float Turbidity { get; set; }
        public float Chlorophyll { get; set; }
        public float BlueGreenAlgae { get; set; }
    }
}
