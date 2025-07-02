using Reporting.Application.DTOs;

namespace Reporting.Application.Interfaces
{
    public interface IReportingService
    {
        Task<int> GetNombreConsultationsAsync(DateTime dateDebut, DateTime dateFin);

        Task<IEnumerable<RendezVousStatDTO>> GetStatistiquesRendezVousAsync(DateTime dateDebut, DateTime dateFin);

        Task<int> GetNombreNouveauxPatientsAsync(DateTime dateDebut, DateTime dateFin);

        Task<List<DoctorStatsDTO>> GetNombreMedecinParSpecialite();
        Task<List<DoctorStatsDTO>> GetNombreMedecinByClinique();
        Task<List<DoctorStatsDTO>> GetNombreMedecinBySpecialiteDansUneClinique(Guid cliniqueId);

        Task<List<FactureStatsDTO>> GetNombreDeFactureByStatus();
        Task<List<FactureStatsDTO>> GetNombreDeFactureParClinique();
        Task<List<FactureStatsDTO>> GetNombreDeFactureParStatusParClinique();
        Task<List<FactureStatsDTO>> GetNombreDeFacturesByStatusDansUneClinique(Guid cliniqueId);


        Task<int> GetNombreDeCliniques();
        Task<int> GetNombreNouvellesCliniquesDuMois();
        Task<IEnumerable<StatistiqueDTO>> GetNombreNouvellesCliniquesParMois();
        Task<StatistiqueCliniqueDTO> GetStatistiquesClinique(Guid cliniqueId);
        Task<IEnumerable<ActiviteMedecinDTO>> GetActivitesMedecin(Guid medecinId);


        Task<decimal> GetMontantPaiementsAsync(string statut, DateTime dateDebut, DateTime dateFin);
        Task<StatistiquesFactureDto> GetStatistiquesFacturesAsync(DateTime dateDebut, DateTime dateFin);
        Task<List<ComparaisonCliniqueDTO>> ComparerCliniquesAsync(List<Guid> cliniqueIds);
        Task<DashboardStatsDTO> GetDashboardStatsAsync(DateTime dateDebut, DateTime dateFin);
    }
}
