// ReSharper disable InconsistentNaming
namespace Under_the_Bay.API.Jobs
{
    public class SampleRecord
    {
        public string ThreeLetter {get; set; }
        public string StationName {get; set; }
        public string StationID {get; set; }
        public string SampleDate {get; set; }
        public string SampleTime {get; set; }
        public string Layer {get; set; }
        public float? SampleDepth_m {get; set; }
        public float? Temp_C {get; set; }
        public float? Salinity_ppt {get; set; }
        public float? DO_mgL {get; set; }
        public float? DO_sat {get; set; }
        public float? pH {get; set; }
        public float? Turbidity_NTU {get; set; }
        public float? ChlA_ugL {get; set; }
        public float? BGA_RFU {get; set; }
        public string Status {get; set; }
    }
}