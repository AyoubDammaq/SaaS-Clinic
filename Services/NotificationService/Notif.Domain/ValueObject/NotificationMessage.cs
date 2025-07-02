using Notif.Domain.Enums;

namespace Notif.Domain.ValueObject
{
    public class NotificationMessage
    {
        public Guid RecipientId { get; set; }
        public UserType RecipientType { get; set; } = UserType.Patient; // par défaut, on envoie aux patients
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public NotificationChannel Channel { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; } // utile si tu ajoutes SMS plus tard
        //public UserNotificationPreferences UserPreferences { get; set; } = new();
    }
}
