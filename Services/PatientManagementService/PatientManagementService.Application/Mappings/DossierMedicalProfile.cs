using AutoMapper;
using PatientManagementService.Application.DTOs;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.Mappings
{
    public class DossierMedicalProfile : Profile
    {
        public DossierMedicalProfile()
        {
            CreateMap<DossierMedicalDTO, DossierMedical>()
                .ForMember(dest => dest.DateCreation, opt => opt.Ignore()); // Géré côté handler
            CreateMap<DossierMedical, DossierMedicalDTO>();
        }
    }
}
