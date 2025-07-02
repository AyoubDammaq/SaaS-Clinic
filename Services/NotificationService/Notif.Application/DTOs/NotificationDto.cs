using Notif.Domain.Enums;

namespace Notif.Application.DTOs
{
    public record NotificationDto(
        Guid Id,
        Guid RecipientId,
        UserType RecipientType,
        NotificationType Type,
        NotificationChannel Channel,
        string Title,
        string Content,
        NotificationPriority Priority,
        NotificationStatus Status,
        DateTime CreatedAt,
        DateTime? SentAt);
}
