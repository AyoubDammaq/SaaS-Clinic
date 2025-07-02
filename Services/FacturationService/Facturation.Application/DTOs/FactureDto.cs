using Facturation.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Facturation.Application.DTOs
{
    public class FactureDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid ConsultationId { get; set; }

        [Required]
        public Guid ClinicId { get; set; }

        public DateTime DateEmission { get; set; } = DateTime.Now;

        [Range(0, double.MaxValue, ErrorMessage = "Le montant total doit être positif.")]
        public decimal MontantTotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le montant total doit être positif.")]
        public decimal MontantPaye { get; set; }

        [EnumDataType(typeof(FactureStatus))]
        public FactureStatus Status { get; set; }
    }
}
