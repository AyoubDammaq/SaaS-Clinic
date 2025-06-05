using MediatR;

namespace Doctor.Domain.Events.DisponibilityEvents
{
    public class DisponibiliteModifiee : INotification
    {
        public Guid DisponibiliteId { get; }
        public Guid MedecinId { get; }

        public DisponibiliteModifiee(Guid disponibiliteId, Guid medecinId)
        {
            DisponibiliteId = disponibiliteId;
            MedecinId = medecinId;
        }
    }
}
