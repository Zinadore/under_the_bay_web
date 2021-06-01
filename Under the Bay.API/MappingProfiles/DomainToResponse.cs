using System;
using System.Globalization;
using Under_the_Bay.API.V1.Contracts.Responses;
using Under_the_Bay.Data.Models;
using AutoMapper;
using NodaTime;

namespace Under_the_Bay.API.MappingProfiles
{
    public class DomainToResponse: Profile
    {
        public DomainToResponse()
        {
            CreateMap<Instant, DateTimeOffset>()
                .ConvertUsing<InstantToOffsetTypeConverter>();
            
            CreateMap<Station, StationResponse>()
                .ForMember(response => response.Samples, opt => opt.MapFrom(s => s.Samples));

            CreateMap<Sample, SampleResponse>(); 
        }
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