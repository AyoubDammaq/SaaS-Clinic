using RDV.Domain.Common;
using RDV.Domain.Entities;

namespace RDV.Domain.Events
{
    public class RendezVousModifie : BaseDomainEvent
    {
        public RendezVous RendezVous { get; }

        public RendezVousModifie(RendezVous rendezVous)
        {
            RendezVous = rendezVous;
        }
    }
}
