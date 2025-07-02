using AutoMapper;
using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Domain.Entities;

namespace ConsultationManagementService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ConsultationDTO, Consultation>().ReverseMap();
            CreateMap<DocumentMedicalDTO, DocumentMedical>().ReverseMap();
        }
    }
}
