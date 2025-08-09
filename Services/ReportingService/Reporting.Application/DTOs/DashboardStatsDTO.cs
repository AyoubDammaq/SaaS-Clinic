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
    }
}
