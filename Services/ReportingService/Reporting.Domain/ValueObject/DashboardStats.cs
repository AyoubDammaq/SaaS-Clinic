using Reporting.Infrastructure.Repositories;

namespace Reporting.Domain.ValueObject
{
    public class DashboardStats
    {
        public int ConsultationsJour { get; set; }
        public int TotalPatients { get; set; }  
        public int NouveauxPatients { get; set; }
        public IEnumerable<Statistique>? NouveauxPatientsParMois { get; set; }
        public int TotalMedecins { get; set; }
        public int NouveauxMedecins { get; set; }
        public int NombreFactures { get; set; }
        public decimal TotalFacturesPayees { get; set; }
        public decimal TotalFacturesImpayees { get; set; }
        public decimal PaiementsPayes { get; set; }
        public decimal PaiementsImpayes { get; set; }
        public decimal PaiementsEnAttente { get; set; }
        public IEnumerable<RendezVousStat>? RendezvousStats { get; set; } 
        public IEnumerable<DoctorStats>? DoctorsBySpecialty { get; set; }
        public int TotalClinics { get; set; }
        public int NouvellesCliniques { get; set; }
        public IEnumerable<AppointmentDayStat>? WeeklyAppointmentStatsByDoctor { get; set; } 
        public IEnumerable<AppointmentDayStat>? WeeklyAppointmentStatsByClinic { get; set; }
    }
}
