using Facturation.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Facturation.Domain.Entities
{
    public class Paiement
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Montant { get; set; }
        public DateTime DatePaiement { get; set; }

        [Required]
        public ModePaiement Mode { get; set; }

        [ForeignKey("Facture")]
        [Required]
        public Guid FactureId { get; set; }
        public virtual Facture? Facture { get; set; }
    }
}
