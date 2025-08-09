using Facturation.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Facturation.Domain.Common;

namespace Facturation.Domain.Entities
{
    public class Paiement : BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Montant { get; set; }

        [Required]
        public DateTime DatePaiement { get; set; } = DateTime.Now;

        [Required]
        public ModePaiement Mode { get; set; }

        [ForeignKey("Facture")]
        [Required]
        public Guid FactureId { get; set; }
        public virtual Facture? Facture { get; set; }

        public virtual CardDetails? CardDetails { get; set; }

        public void PayerFactureEvent()
        {
            AddDomainEvent(new Events.FacturePayed(this));
        }
    }
}
