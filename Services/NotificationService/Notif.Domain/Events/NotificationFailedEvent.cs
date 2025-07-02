using Notif.Domain.Common;
using Notif.Domain.Enums;

namespace Notif.Domain.Events
{
    public record NotificationFailedEvent(
        Guid NotificationId,
        Guid RecipientId,
        NotificationChannel Channel,
        string ErrorMessage,
        int RetryCount) : IDomainEvent;
}
