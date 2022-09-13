using AutoMapper;
using NodaTime;
using System;
using System.Globalization;
using UTB.API.V1.Contracts.Responses;
using UTB.Contracts.DTO;

namespace UTB.API.MappingProfiles
{
    public sealed class DomainMappings : Profile
    {
        public DomainMappings()
        {
            CreateMap<Instant, DateTimeOffset>()
                .ConvertUsing<InstantToOffsetTypeConverter>();

            CreateMap<SampleDTO, SampleResponse>();

            CreateMap<StationDTO, StationResponse>()
                .ForMember(dest => dest.Samples, opts => opts.MapFrom(src => src.Samples));
        }

        public class InstantToOffsetTypeConverter : ITypeConverter<Instant, DateTimeOffset>
        {
            private static readonly DateTimeZone TimeZone = DateTimeZoneProviders.Tzdb["America/New_York"];

            public DateTimeOffset Convert(Instant source, DateTimeOffset destination, ResolutionContext context)
            {
                var format = $"yyyy/MM/dd HH:mm:ss '{TimeZone.GetUtcOffset(source)}'";
                var str = source.InZone(TimeZone).ToString(format, CultureInfo.InvariantCulture);
                if (DateTimeOffset.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                {
                    return result;
                }
                return DateTimeOffset.MinValue;
            }
        }
    }
}
