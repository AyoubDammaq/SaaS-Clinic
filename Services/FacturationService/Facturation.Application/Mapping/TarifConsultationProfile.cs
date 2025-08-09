using AutoMapper;
using Facturation.Application.DTOs;
using Facturation.Domain.Entities;

namespace Facturation.Application.Mapping
{
    public class TarifConsultationProfile : Profile
    {
        public TarifConsultationProfile()
        {
            CreateMap<TarifConsultation, TarifConsultationDto>();
            CreateMap<TarifConsultationDto, TarifConsultation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<AddTarificationRequest, TarifConsultation>();
            CreateMap<UpdateTarificationRequest, TarifConsultation>();
        }
    }
}
