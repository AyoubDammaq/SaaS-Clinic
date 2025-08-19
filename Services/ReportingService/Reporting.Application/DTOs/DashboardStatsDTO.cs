using Facturation.Application.DTOs;
using Reporting.Domain.ValueObject;

namespace Reporting.Application.DTOs
{
    public class DashboardStatsDTO
    {
        public int ConsultationsJour { get; set; }
        public int TotalPatients { get; set; }
        public int NouveauxPatients { get; set; }
        public IEnumerable<StatistiqueDTO>? NouveauxPatientsParMois { get; set; } 
        public int TotalMedecins { get; set; }
        public int NouveauxMedecins { get; set; }
        public int NombreFactures { get; set; }
        public decimal TotalFacturesPayees { get; set; }
        public decimal TotalFacturesImpayees { get; set; }
        public decimal PaiementsPayes { get; set; }
        public decimal PaiementsImpayes { get; set; }
        public decimal PaiementsEnAttente { get; set; }
        public IEnumerable<RendezVousStatDTO>? RendezvousStats { get; set; }
        public IEnumerable<DoctorStatsDTO>? DoctorsBySpecialty { get; set; }
        public int TotalClinics { get; set; }
        public int NouvellesCliniques { get; set; }
        public IEnumerable<AppointmentDayStatDto>? WeeklyAppointmentStatsByDoctor { get; set; }
        public IEnumerable<AppointmentDayStatDto>? WeeklyAppointmentStatsByClinic { get; set; }

        public int NombreDeConsultationsParPatient { get; set; }
        public RecentPaiement? RecentPaiementByPatient { get; set; }
        public int NouveauxPatientsParMedecin { get; set; }
        public int NouveauxPatientsParClinic { get; set; }
        public int NombreDeConsultationsMensuellesParDoctor { get; set; }
        public int NombreDeConsultationsMensuellesParClinic { get; set; }
        public Trend? TrendDeNouveauxPatientsMensuelsParMedecin { get; set; }
        public Trend? TrendDeNouveauxPatientsMensuelsParClinic { get; set; }
        public Trend? TrendDeConsultationsMensuellesParDoctor { get; set; }
        public Trend? TrendDeConsultationsMensuellesParClinic { get; set; }
        public int NombreDeRDVParMedecinAujourdHui { get; set; }
        public int NombreDeRDVParCliniqueAujourdHui { get; set; }
        public int NombreDePendingAppointmentsByDoctor { get; set; }
        public int NombreDePendingAppointmentsByClinic { get; set; }
        public decimal RevenuesMensuelsByClinic { get; set; }
        public RevenusMensuelTrendDto? RevenuesMensuelsByClinicTrend { get; set; }
        public Trend? TrendDeNouvellesCliniques { get; set; }
        public Trend? TrendDeNouveauxMedecins { get; set; }
        public Trend? TrendDeNouveauxPatients { get; set; }
    }
}
