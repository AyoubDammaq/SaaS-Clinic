using RDVManagementService.Models;

public class RendezVousDTO
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public Guid MedecinId { get; set; }
    public DateTime DateHeure { get; set; }
    public string Commentaire { get; set; } = string.Empty;
}

