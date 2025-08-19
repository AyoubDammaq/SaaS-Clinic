namespace Clinic.Application.DTOs
{
    public class StatistiqueCliniqueDTO
    {
        public Guid CliniqueId { get; set; }
        public string Nom { get; set; }
        public int NombreMedecins { get; set; }
        public int NombreRendezVous { get; set; }
        public int NombreConsultations { get; set; }
        public int NombrePatients { get; set; }

        public Dictionary<int, int> NombreConsultationsParMois { get; set; } = new();
        public Dictionary<int, int> NombreNouveauxPatientsParMois { get; set; } = new();
        public Dictionary<int, decimal> RevenusParMois { get; set; } = new();
    }
}
