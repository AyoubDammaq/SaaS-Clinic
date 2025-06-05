using Facturation.Domain.Common;
using Facturation.Domain.Entities;

namespace Facturation.Domain.Events
{
    public class FacturePayed : BaseDomainEvent
    {
        public Paiement Paiement { get; }
        public FacturePayed(Paiement paiement)
        {
            Paiement = paiement ?? throw new ArgumentNullException(nameof(paiement));
        }
    }
}
