using System;
using System.ComponentModel.DataAnnotations;

namespace Under_the_Bay.Data.Models
{
    public class Sample
    {
        public Guid Id { get; set; }
        [Required]
        public DateTimeOffset SampleDate { get; set; }
        public float SampleDepth { get; set; }
        public float WaterTemperature { get; set; }
        public float DissolvedOxygen { get; set; }
        public float DissolvedOxygenSaturation { get; set; }
        public float Salinity { get; set; }
        // ReSharper disable once InconsistentNaming
        [Range(0, 14)]
        public float PH { get; set; }
        public float Turbidity { get; set; }
        public float Chlorophyll { get; set; }
        public float BlueGreenAlgae { get; set; }
        
        public Guid StationId { get; set; }
        public virtual Station Station { get; set; }
    }
}