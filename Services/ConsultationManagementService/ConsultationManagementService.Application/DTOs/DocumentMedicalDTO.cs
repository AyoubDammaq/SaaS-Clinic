using System.ComponentModel.DataAnnotations;

namespace ConsultationManagementService.Application.DTOs
{
    public class DocumentMedicalDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ConsultationId { get; set; }

        [Required]
        [StringLength(100)] 
        public string FileName { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;  // Ex: "Ordonnance", "Radio", "Analyse", etc.

        [Required]
        [Url]
        public string FichierURL { get; set; } = string.Empty;

        public DateTime DateAjout { get; set; } = DateTime.Now;
    }
}
