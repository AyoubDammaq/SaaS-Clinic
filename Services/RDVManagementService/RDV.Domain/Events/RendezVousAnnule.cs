using RDV.Domain.Common;

namespace RDV.Domain.Events
{
    public class RendezVousAnnule : BaseDomainEvent
    {
        public Guid RendezVousId { get; }

        public RendezVousAnnule(Guid rendezVousId)
        {
            RendezVousId = rendezVousId;
        }
    }
}
