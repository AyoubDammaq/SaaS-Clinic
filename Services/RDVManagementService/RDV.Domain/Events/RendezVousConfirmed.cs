using RDV.Domain.Common;
using RDV.Domain.Entities;

namespace RDV.Domain.Events
{
    public class RendezVousConfirmed : BaseDomainEvent
    {
        public RendezVous RendezVous { get; }

        public RendezVousConfirmed(RendezVous rendezVous)
        {
            RendezVous = rendezVous;
        }
    }
}
