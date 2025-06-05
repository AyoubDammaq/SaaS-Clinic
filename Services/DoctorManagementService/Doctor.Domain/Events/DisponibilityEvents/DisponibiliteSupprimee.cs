using MediatR;

namespace Doctor.Domain.Events.DisponibilityEvents
{
    public class DisponibiliteSupprimee : INotification
    {
        public Guid DisponibiliteId { get; }

        public DisponibiliteSupprimee(Guid disponibiliteId)
        {
            DisponibiliteId = disponibiliteId;
        }
    }
}
