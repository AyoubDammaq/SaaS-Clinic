namespace RDV.Application.DTOs
{
    public class CreateRendezVousDto
    {
        public Guid PatientId { get; set; }

        public Guid MedecinId { get; set; }

        public DateTime DateHeure { get; set; }

        public string? Commentaire { get; set; }
    }
}
