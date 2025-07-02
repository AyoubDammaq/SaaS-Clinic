using AutoMapper;
using Reporting.Application.DTOs;
using Reporting.Domain.Entities;
using Reporting.Domain.ValueObject;
using Reporting.Infrastructure.Repositories;

namespace Reporting.Application.Mapping
{
    public class ReportingMappingProfile : Profile
    {
        public ReportingMappingProfile()
        {
            CreateMap<RendezVousStat, RendezVousStatDTO>();
            CreateMap<DoctorStats, DoctorStatsDTO>();
            CreateMap<FactureStats, FactureStatsDTO>();
            CreateMap<Statistique, StatistiqueDTO>();
            CreateMap<StatistiqueClinique, StatistiqueCliniqueDTO>();
            CreateMap<ActiviteMedecin, ActiviteMedecinDTO>();
            CreateMap<StatistiqueClinique, ComparaisonCliniqueDTO>();
            CreateMap<DashboardStats, DashboardStatsDTO>();
        }
    }
}
