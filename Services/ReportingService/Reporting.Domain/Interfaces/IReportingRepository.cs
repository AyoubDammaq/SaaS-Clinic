

using Reporting.Domain.ValueObject;
using Reporting.Infrastructure.Repositories;

namespace Reporting.Domain.Interfaces
{
    public interface IReportingRepository
    {
        Task<int> GetNombreConsultationsAsync(DateTime dateDebut, DateTime dateFin);

        Task<IEnumerable<RendezVousStat>> GetStatistiquesRendezVousAsync(DateTime dateDebut, DateTime dateFin);

        Task<int> GetNombreNouveauxPatientsAsync(DateTime dateDebut, DateTime dateFin);
        Task<int> GetNombreDeNouvellesCliniquesAsync(DateTime dateDebut, DateTime dateFin);
        Task<int> GetNombreNouveauxMedecinsAsync(DateTime dateDebut, DateTime dateFin);

        Task<List<DoctorStats>> GetNombreMedecinParSpecialiteAsync();
        Task<List<DoctorStats>> GetNombreMedecinByCliniqueAsync();
        Task<List<DoctorStats>> GetNombreMedecinBySpecialiteDansUneCliniqueAsync(Guid cliniqueId);

        Task<List<FactureStats>> GetNombreDeFactureByStatusAsync();
        Task<List<FactureStats>> GetNombreDeFactureParCliniqueAsync();
        Task<List<FactureStats>> GetNombreDeFacturesByStatusParCliniqueAsync();
        Task<List<FactureStats>> GetNombreDeFacturesByStatusDansUneCliniqueAsync(Guid cliniqueId);

        Task<int> GetNombreDeCliniquesAsync();
        Task<int> GetNombreNouvellesCliniquesDuMoisAsync();
        Task<IEnumerable<Statistique>> GetNombreNouvellesCliniquesParMoisAsync();
        Task<StatistiqueClinique> GetStatistiquesCliniqueAsync(Guid cliniqueId);
        Task<IEnumerable<ActiviteMedecin>> GetActivitesMedecinAsync(Guid medecinId);


        Task<decimal> GetMontantPaiementsAsync(string statut, DateTime dateDebut, DateTime dateFin);
        Task<StatistiquesFacture> GetStatistiquesFacturesAsync(DateTime dateDebut, DateTime dateFin);
        Task<IEnumerable<AppointmentDayStat>> GetStatistiquesHebdomadairesRendezVousByDoctorAsync(Guid medecinId, DateTime dateDebut, DateTime dateFin);
        Task<IEnumerable<AppointmentDayStat>> GetStatistiquesHebdomadairesRendezVousByClinicAsync(Guid cliniqueId, DateTime dateDebut, DateTime dateFin);
        Task<(int trendValue, bool isPositive)> CalculerTrendDeConsultationsMensuellesParDoctorAsync(Guid medecinId);
        Task<(int trendValue, bool isPositive)> CalculerTrendDeConsultationsMensuellesParClinicAsync(Guid cliniqueId);
        Task<Trend> CalculerTrendDeNouveauxPatientsMensuelsParMedecinAsync(Guid medecinId);
        Task<Trend> CalculerTrendDeNouveauxPatientsMensuelsParCliniqueAsync(Guid cliniqueId);
        Task<Trend> CalculerNewClinicsTrend(DateTime dateDebut, DateTime dateFin);
        Task<Trend> CalculerNewDoctorsTrend(DateTime dateDebut, DateTime dateFin);
        Task<Trend> CalculerNewPatientsTrend(DateTime dateDebut, DateTime dateFin);
        Task<int> CalculerNombreDeRDVParMedecinAujourdhuiAsync(Guid medecinId);
        Task<DashboardStats> GetDashboardStatsAsync(DateTime dateDebut, DateTime dateFin, Guid? patientId = null, Guid? medecinId = null, Guid? cliniqueId = null);
    }
}
