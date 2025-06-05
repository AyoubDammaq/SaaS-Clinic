using RDV.Domain.Common;

namespace RDV.Domain.Events
{
    public class RendezVousAnnuleParMedecin : BaseDomainEvent
    {
        public Guid RendezVousId { get; }
        public string Raison { get; }

        public RendezVousAnnuleParMedecin(Guid rendezVousId, string raison)
        {
            RendezVousId = rendezVousId;
            Raison = raison;
        }
    }
}
