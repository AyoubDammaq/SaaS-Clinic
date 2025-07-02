using System.ComponentModel.DataAnnotations;

namespace Facturation.Application.DTOs
{
    public class CreateFactureRequest
    {
        [Required(ErrorMessage = "Le PatientId est requis.")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Le ConsultationId est requis.")]
        public Guid ConsultationId { get; set; }

        [Required(ErrorMessage = "Le ClinicId est requis.")]
        public Guid ClinicId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Le MontantTotal doit être supérieur à 0.")]
        public decimal MontantTotal { get; set; }
    }
}
