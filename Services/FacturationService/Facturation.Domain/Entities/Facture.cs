using Facturation.Domain.Common;
using Facturation.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Facturation.Domain.Entities
{
    public class Facture : BaseEntity
    {
        [Key]
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

        public virtual Paiement? Paiement { get; set; }

        public void CreateFactureEvent()
        {
            AddDomainEvent(new Events.FactureCreated(this));
        }

        public void UpdateFactureEvent()
        {
            AddDomainEvent(new Events.FactureUpdated(this));
        }

        public void DeleteFactureEvent()
        {
            AddDomainEvent(new Events.FactureDeleted(this.Id));
        }
    }
}
