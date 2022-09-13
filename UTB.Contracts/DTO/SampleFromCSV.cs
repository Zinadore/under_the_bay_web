// ReSharper disable InconsistentNaming
namespace UTB.Contracts.DTO
{
    public class SampleFromCSV
    {
        public string ThreeLetter { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string StationID { get; set; } = string.Empty;
        public string SampleDate { get; set; } = string.Empty;
        public string SampleTime { get; set; } = string.Empty;
        public string Layer { get; set; } = string.Empty;
        public float? SampleDepth_m { get; set; }
        public float? Temp_C { get; set; }
        public float? Salinity_ppt { get; set; }
        public float? DO_mgL { get; set; }
        public float? DO_sat { get; set; }
        public float? pH { get; set; }
        public float? Turbidity_NTU { get; set; }
        public float? ChlA_ugL { get; set; }
        public float? BGA_RFU { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}