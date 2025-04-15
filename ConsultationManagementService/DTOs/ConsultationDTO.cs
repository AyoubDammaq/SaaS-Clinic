using System.ComponentModel.DataAnnotations;

namespace ConsultationManagementService.DTOs
{
    public class ConsultationDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid MedecinId { get; set; }

        [Required]
        public DateTime DateConsultation { get; set; }

        public string Diagnostic { get; set; }

        public string Notes { get; set; }

    }
}
