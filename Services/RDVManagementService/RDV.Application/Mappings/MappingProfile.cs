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
            CreateMap<CreateRendezVousDto, RendezVous>()
                .ForMember(dest => dest.Statut, opt => opt.Ignore())        // sera défini manuellement
                .ForMember(dest => dest.DateCreation, opt => opt.Ignore()); // défini par défaut
            // DateCreation est générée automatiquement côté entity

            // Optionnel : Mapping inverse Entity -> DTO
            CreateMap<RendezVous, RendezVousDTO>();
        }
    }
}
