namespace PatientManagementService.Application.DTOs
{
    public class DocumentDTO
    {
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public string Url{ get; set; }
        public string Type { get; set; }
        public DateTime DateCreation { get; set; }
    }
}