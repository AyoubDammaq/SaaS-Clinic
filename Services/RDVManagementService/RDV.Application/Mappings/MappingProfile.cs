using AutoMapper;
using RDV.Application.DTOs;
using RDV.Domain.Entities;

namespace RDV.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping DTO -> Entity
            CreateMap<RendezVousDTO, RendezVous>()
                .ForMember(dest => dest.DateCreation, opt => opt.Ignore());
            // DateCreation est générée automatiquement côté entity

            // Optionnel : Mapping inverse Entity -> DTO
            CreateMap<RendezVous, RendezVousDTO>();
        }
    }
}
