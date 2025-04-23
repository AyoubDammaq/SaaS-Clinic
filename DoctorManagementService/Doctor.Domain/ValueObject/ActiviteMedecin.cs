
namespace Doctor.Domain.ValueObject
{
    public class ActiviteMedecin
    {
        public Guid MedecinId { get; set; }
        public string NomComplet { get; set; }
        public int NombreConsultations { get; set; }
        public int NombreRendezVous { get; set; }
    }
}
