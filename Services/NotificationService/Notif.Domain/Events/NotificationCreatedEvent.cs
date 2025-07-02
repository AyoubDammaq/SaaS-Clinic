using Notif.Domain.Common;
using Notif.Domain.Enums;

namespace Notif.Domain.Events
{
    public record NotificationCreatedEvent(
        Guid NotificationId,
        Guid RecipientId,
        UserType RecipientType,
        NotificationType Type,
        NotificationChannel Channel,
        NotificationPriority Priority,
        DateTime CreatedAt) : IDomainEvent;
}
