using Under_the_Bay.API.V1.Contracts.Responses;
using Under_the_Bay.Data.Models;
using AutoMapper;

namespace Under_the_Bay.API.MappingProfiles
{
    public class DomainToResponse: Profile
    {
        public DomainToResponse()
        {
            CreateMap<Station, StationResponse>()
                .ForMember(response => response.Samples, opt => opt.MapFrom(s => s.Samples));

            CreateMap<Sample, SampleResponse>();
        }
    }
}