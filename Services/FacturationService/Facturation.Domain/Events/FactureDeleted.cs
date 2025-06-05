using Facturation.Domain.Common;

namespace Facturation.Domain.Events
{
    public class FactureDeleted : BaseDomainEvent
    {
        public Guid FactureId { get; }
        public FactureDeleted(Guid factureId)
        {
            FactureId = factureId;
        }
    }
}
