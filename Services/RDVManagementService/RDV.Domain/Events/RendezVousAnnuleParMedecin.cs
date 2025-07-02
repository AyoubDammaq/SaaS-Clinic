using RDV.Domain.Common;
using RDV.Domain.Entities;

namespace RDV.Domain.Events
{
    public class RendezVousAnnuleParMedecin : BaseDomainEvent
    {
        public RendezVous RendezVous { get; }
        public string Raison { get; }

        public RendezVousAnnuleParMedecin(RendezVous rendezVous, string raison)
        {
            RendezVous = rendezVous;
            Raison = raison;
        }
    }
}
