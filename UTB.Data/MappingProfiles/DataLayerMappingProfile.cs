using AutoMapper;
using NodaTime;
using System;
using System.Globalization;
using UTB.Contracts.DTO;
using UTB.Data.Models;

namespace UTB.Data.MappingProfiles
{
    public class DataLayerMappingProfile : Profile
    {
        public DataLayerMappingProfile()
        {
            //CreateMap<SampleFromCSV, Sample>()
            //    .ForMember(destination => destination.SampleDepth,
            //        opts => opts.ConvertUsing(new ValueClamper(0, 200), source => source.SampleDepth_m))
            //    .ForMember(destination => destination.WaterTemperature,
            //        opts => opts.ConvertUsing(new ValueClamper(0, 100), source => source.Temp_C))
            //    .ForMember(destination => destination.DissolvedOxygen,
            //        opts => opts.ConvertUsing(new ValueClamper(0, 21), source => source.DO_mgL))
            //    .ForMember(destination => destination.DissolvedOxygenSaturation,
            //        opts => opts.ConvertUsing(new ValueClamper(0, 1), source => source.DO_sat))
            //    .ForMember(destination => destination.Salinity,
            //        opts => opts.ConvertUsing(new ValueClamper(0, 32), source => source.Salinity_ppt))
            //    .ForMember(destination => destination.pH,
            //        opts => opts.ConvertUsing(new ValueClamper(0, 14), source => source.pH))
            //    .ForMember(destination => destination.Turbidity,
            //        opts => opts.ConvertUsing(new ValueClamper(0, 100), source => source.Turbidity_NTU))
            //    .ForMember(destination => destination.Chlorophyll,
            //        opts => opts.ConvertUsing(new ValueClamper(0, 100), source => source.ChlA_ugL))
            //    .ForMember(destination => destination.BlueGreenAlgae,
            //        opts => opts.ConvertUsing(new ValueClamper(0, 100), source => source.BGA_RFU))
            //    ;

            CreateMap<Station, StationDTO>()
                .ForMember(d => d.Samples, opts => opts.MapFrom(s => s.Samples));
            CreateMap<Sample, SampleDTO>();


        }
    }

    //public class ValueClamper : IValueConverter<float?, float>
    //{
    //    private readonly float min;
    //    private readonly float max;

    //    public ValueClamper(float min, float max)
    //    {
    //        this.min = min;
    //        this.max = max;
    //    }
    //    public float Convert(float? sourceMember, ResolutionContext context)
    //    {
    //        return sourceMember != null ? Math.Clamp(sourceMember.Value, min, max) : min;
    //    }
    //}
}
