using AutoMapper;
using PatientManagementService.Application.DTOs;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.Mappings
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            // 1. Mapping pour la création (POST)
            CreateMap<PatientDTO, Patient>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // ✅ Ignore l'id si généré par le backend

            // 2. Mapping pour la lecture (GET)
            CreateMap<Patient, PatientDTO>();

            // 3. Mapping pour CreatePatientDTO (soumission de formulaire)
            CreateMap<CreatePatientDTO, Patient>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())               // ✅ généré en base
                .ForMember(dest => dest.DateCreation, opt => opt.Ignore());   // ✅ initialisé côté service/domaine
        }
    }
}
