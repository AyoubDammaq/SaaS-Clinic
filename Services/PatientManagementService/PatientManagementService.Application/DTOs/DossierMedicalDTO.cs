namespace PatientManagementService.Application.DTOs
{
    public class DossierMedicalDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PatientId { get; set; }
        public string Allergies { get; set; } = string.Empty;
        public string MaladiesChroniques { get; set; } = string.Empty;
        public string MedicamentsActuels { get; set; } = string.Empty;
        public string AntécédentsFamiliaux { get; set; } = string.Empty;
        public string AntécédentsPersonnels { get; set; } = string.Empty;
        public string GroupeSanguin { get; set; } = string.Empty;
        public List<DocumentDTO>? Documents { get; set; }
    }

}
