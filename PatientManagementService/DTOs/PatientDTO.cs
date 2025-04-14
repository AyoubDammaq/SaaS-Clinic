namespace PatientManagementService.DTOs
{
    public class PatientDTO
    {
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public DateTime DateNaissance { get; set; }
        public string Sexe { get; set; }
        public string Adresse { get; set; }
        public string NumeroTelephone { get; set; }
        public string Email { get; set; }
    }
}
