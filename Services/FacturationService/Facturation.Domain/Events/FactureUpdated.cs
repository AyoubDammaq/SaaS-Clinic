using Facturation.Domain.Common;
using Facturation.Domain.Entities;

namespace Facturation.Domain.Events
{
    public class FactureUpdated : BaseDomainEvent
    {
        public Facture Facture { get; }

        public FactureUpdated(Facture facture)
        {
            Facture = facture ?? throw new ArgumentNullException(nameof(facture));
        }
    }
}
