using RDV.Domain.Common;
using RDV.Domain.Entities;

namespace RDV.Domain.Events
{
    public class RendezVousAnnule : BaseDomainEvent
    {
        public RendezVous RendezVous { get; }

        public RendezVousAnnule(RendezVous rendezVous)
        {
            RendezVous = rendezVous;
        }
    }
}
