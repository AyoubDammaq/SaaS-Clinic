using Notif.Domain.Enums;

namespace Notif.Application.DTOs
{
    public record CreateNotificationRequest(
        Guid RecipientId,
        UserType RecipientType,
        NotificationType Type,
        string Title,
        string Content,
        NotificationPriority Priority = NotificationPriority.Normal,
        DateTime? ScheduledAt = null,
        Dictionary<string, object>? Metadata = null);
}
