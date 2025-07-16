namespace ConsultationManagementService.Application.DTOs
{
    public class MedicalDocumentRemovedDto
    {
        public Guid DocumentId { get; set; }
        public Guid ConsultationId { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}
