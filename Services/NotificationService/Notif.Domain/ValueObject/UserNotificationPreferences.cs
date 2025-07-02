using Notif.Domain.Enums;

namespace Notif.Domain.ValueObject
{
    public class UserNotificationPreferences
    {
        public Guid UserId { get; set; }
        public UserType UserType { get; set; }
        public List<NotificationChannel> PreferredChannels { get; set; } = new();
        public Dictionary<NotificationType, bool> NotificationSettings { get; set; } = new();
        public string Language { get; set; } = "fr";
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
