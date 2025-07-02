namespace Reporting.Application.DTOs
{
    public class ComparaisonCliniqueDTO
    {
        public Guid CliniqueId { get; set; }
        public string Nom { get; set; }
        public int NombreMedecins { get; set; }
        public int NombrePatients { get; set; }
        public int NombreConsultations { get; set; }
        public int NombreRendezVous { get; set; }
    }
}
