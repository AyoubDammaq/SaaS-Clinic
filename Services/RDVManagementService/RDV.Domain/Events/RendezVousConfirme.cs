using RDV.Domain.Common;

namespace RDV.Domain.Events
{
    public class RendezVousConfirme : BaseDomainEvent
    {
        public Guid RendezVousId { get; }

        public RendezVousConfirme(Guid rendezVousId)
        {
            RendezVousId = rendezVousId;
        }
    }
}
