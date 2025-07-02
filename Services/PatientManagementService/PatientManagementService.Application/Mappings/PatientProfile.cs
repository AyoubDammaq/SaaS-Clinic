using AutoMapper;
using PatientManagementService.Application.DTOs;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.Mappings
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<PatientDTO, Patient>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // géré côté création
            CreateMap<Patient, PatientDTO>();
        }
    }
}
