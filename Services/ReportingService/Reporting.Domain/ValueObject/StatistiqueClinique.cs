namespace Reporting.Infrastructure.Repositories
{
    public class StatistiqueClinique
    {
        public Guid CliniqueId { get; set; }
        public string Nom { get; set; }
        public int NombreMedecins { get; set; }
        public int NombreRendezVous { get; set; }
        public int NombreConsultations { get; set; }
        public int NombrePatients { get; set; }
    }
}