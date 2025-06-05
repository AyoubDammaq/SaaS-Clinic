using MediatR;

namespace Doctor.Domain.Events.DisponibilityEvents
{
    public class DisponibiliteAjoutee : INotification
    {
        public Guid DisponibiliteId { get; }
        public Guid MedecinId { get; }

        public DisponibiliteAjoutee(Guid disponibiliteId, Guid medecinId)
        {
            DisponibiliteId = disponibiliteId;
            MedecinId = medecinId;
        }
    }
}
