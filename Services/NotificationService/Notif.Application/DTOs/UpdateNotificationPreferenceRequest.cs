using Notif.Domain.Enums;

namespace Notif.Application.DTOs
{
    public record UpdateNotificationPreferenceRequest(
        Guid UserId,
        UserType UserType,
        List<NotificationChannel> PreferredChannels,
        Dictionary<NotificationType, bool> NotificationSettings,
        string Language = "fr",
        string? PhoneNumber = null,
        string? Email = null
    );
}
