using Facturation.Domain.Common;
using Facturation.Domain.Entities;

namespace Facturation.Domain.Events
{
    public class FactureCreated : BaseDomainEvent
    {
        public Facture Facture { get; }

        public FactureCreated(Facture facture)
        {
            Facture = facture ?? throw new ArgumentNullException(nameof(facture));
        }
    }
}
