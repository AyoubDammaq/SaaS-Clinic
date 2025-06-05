using MediatR;

namespace Doctor.Domain.Events.DisponibilityEvents
{
    public class DisponibilitesSupprimeesParMedecin : INotification
    {
        public Guid MedecinId { get; }

        public DisponibilitesSupprimeesParMedecin(Guid medecinId)
        {
            MedecinId = medecinId;
        }
    }
}
