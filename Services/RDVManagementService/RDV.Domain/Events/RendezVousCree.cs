using RDV.Domain.Common;
using RDV.Domain.Entities;

namespace RDV.Domain.Events
{
    public class RendezVousCree : BaseDomainEvent
    {
        public RendezVous RendezVous { get; }

        public RendezVousCree(RendezVous rendezVous)
        {
            RendezVous = rendezVous;
        }
    }
}
