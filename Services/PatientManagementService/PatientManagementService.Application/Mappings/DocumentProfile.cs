using AutoMapper;
using PatientManagementService.Application.DTOs;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.Mappings
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<CreateDocumentRequest, Document>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreation, opt => opt.Ignore());

            CreateMap<Document, CreateDocumentRequest>();
        }
    }
}
