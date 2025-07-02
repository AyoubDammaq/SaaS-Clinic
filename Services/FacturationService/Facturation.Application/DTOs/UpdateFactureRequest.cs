using System.ComponentModel.DataAnnotations;

namespace Facturation.Application.DTOs
{
    public class UpdateFactureRequest
    {
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid ConsultationId { get; set; }

        [Required]
        public Guid ClinicId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Le montant total doit être supérieur à zéro.")]
        public decimal MontantTotal { get; set; }
    }
}
