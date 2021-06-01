using System;
using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace Under_the_Bay.Data.Models
{
    public class Sample
    {
        public Guid Id { get; set; }
        [Required]
        public Instant SampleDate { get; set; }
        [Range(0, 200)]
        public float SampleDepth { get; set; }
        [Range(0, 100)]
        public float WaterTemperature { get; set; }
        [Range(0, 21)]
        public float DissolvedOxygen { get; set; }
        [Range(0.0f, 1.0f)]
        public float DissolvedOxygenSaturation { get; set; }
        [Range(0.0f, 32.0f)]
        public float Salinity { get; set; }
        // ReSharper disable once InconsistentNaming
        [Range(0, 14)]
        public float pH { get; set; }
        [Range(0, 100)]
        public float Turbidity { get; set; }
        [Range(0, 100)]
        public float Chlorophyll { get; set; }
        [Range(0, 1000)]
        public float BlueGreenAlgae { get; set; }
        
        public Guid StationId { get; set; }
        public virtual Station Station { get; set; }
    }
}