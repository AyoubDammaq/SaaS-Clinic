namespace PatientManagementService.DTOs
{
    public class DossierMedicalDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PatientId { get; set; }
        public string Allergies { get; set; }
        public string MaladiesChroniques { get; set; }
        public string MedicamentsActuels { get; set; }
        public string AntécédentsFamiliaux { get; set; }
        public string AntécédentsPersonnels { get; set; }
        public string GroupeSanguin { get; set; }
    }
        
}
