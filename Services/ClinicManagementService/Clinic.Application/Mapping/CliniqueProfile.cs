using AutoMapper;
using Clinic.Application.DTOs;
using Clinic.Domain.Entities;
using Clinic.Domain.ValueObject;

namespace Clinic.Application.Mapping
{
    public class CliniqueProfile : Profile
    {
        public CliniqueProfile()
        {
            CreateMap<CliniqueDto, Clinique>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); 
            CreateMap<Clinique, CliniqueDto>();
            CreateMap<StatistiqueClinique, StatistiqueCliniqueDTO>();
            CreateMap<Statistique, StatistiqueDTO>();
        }
    }
}
