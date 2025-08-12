using Notif.Domain.Enums;

namespace Notif.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid RecipientId { get; set; }
        public UserType RecipientType { get; set; }
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public NotificationPriority Priority { get; set; }
        public NotificationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }

        public static Notification Create(
            Guid recipientId,
            UserType recipientType,
            NotificationType type,
            NotificationChannel channel,
            string title,
            string content,
            NotificationPriority priority = NotificationPriority.Normal,
            DateTime? scheduledAt = null)
        {
            return new Notification
            {
                Id = Guid.NewGuid(),
                RecipientId = recipientId,
                RecipientType = recipientType,
                Type = type,
                Channel = channel,
                Title = title,
                Content = content,
                Priority = priority,
                Status = NotificationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ScheduledAt = scheduledAt,
                RetryCount = 0,
            };
        }

        public void MarkAsSent()
        {
            Status = NotificationStatus.Sent;
            SentAt = DateTime.UtcNow;
        }

        public void MarkAsFailed(string errorMessage)
        {
            Status = NotificationStatus.Failed;
            ErrorMessage = errorMessage;
            RetryCount++;
        }

        public void Retry()
        {
            Status = NotificationStatus.Pending;
            ErrorMessage = null;
        }

        public bool CanRetry() => RetryCount < 3 && Status == NotificationStatus.Failed;
    }
}
