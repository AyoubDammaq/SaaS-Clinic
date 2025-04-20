using Facturation.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facturation.Domain.Entities
{
    public class Facture 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid ConsultationId { get; set; }

        [Required]
        public Guid ClinicId { get; set; }

        [Required]
        public DateTime DateEmission { get; set; } = DateTime.Now;

        [Required]
        public decimal MontantTotal { get; set; }

        [Required]
        public FactureStatus Status { get; set; }
    }
}
