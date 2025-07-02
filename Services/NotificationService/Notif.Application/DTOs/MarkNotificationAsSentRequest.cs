namespace Notif.Application.DTOs
{
    public record MarkNotificationAsSentRequest(
        Guid NotificationId,
        DateTime SentAt
    );
}
