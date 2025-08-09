using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Facturation.Domain.Entities
{
    public class CardDetails
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string CardholderName { get; set; } = string.Empty;

        [Required]
        [StringLength(19)]
        public string CardNumber { get; set; } = string.Empty; // Stocké sans espaces

        [Required]
        [StringLength(5)]
        public string ExpiryDate { get; set; } = string.Empty; // Format MM/AA

        [Required]
        [StringLength(4)]
        public string Cvv { get; set; } = string.Empty;

        [ForeignKey("Paiement")]
        [Required]
        public Guid PaiementId { get; set; }

        public virtual Paiement? Paiement { get; set; }
    }
}