using System.ComponentModel.DataAnnotations;

namespace ConsultationManagementService.Application.DTOs
{
    public class ConsultationDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid MedecinId { get; set; }

        [Required]
        public Guid ClinicId { get; set; }

        [Required]
        public DateTime DateConsultation { get; set; }

        [Required]
        [StringLength(500)] // Ajout d'une limite raisonnable pour le diagnostic
        public string Diagnostic { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)] // Ajout d'une limite raisonnable pour les notes
        public string Notes { get; set; } = string.Empty;

        public ICollection<DocumentMedicalDTO> Documents { get; set; } = new List<DocumentMedicalDTO>();
    }
}
