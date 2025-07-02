using Notif.Domain.Common;
using Notif.Domain.Enums;

namespace Notif.Domain.Events
{
    public record NotificationSentEvent(
        Guid NotificationId,
        Guid RecipientId,
        NotificationChannel Channel,
        DateTime SentAt) : IDomainEvent;
}
