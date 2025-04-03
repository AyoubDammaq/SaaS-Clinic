namespace DoctorManagementService.DTOs
{
    public class MedecinDto
    {
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Specialite { get; set; }
        public Guid CliniqueId { get; set; }
    }
}
