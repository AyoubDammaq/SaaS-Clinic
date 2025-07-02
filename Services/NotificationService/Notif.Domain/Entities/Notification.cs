using Notif.Domain.Enums;

namespace Notif.Domain.Entities
{

    public class Notification
    {
        public Guid Id { get; private set; }
        public Guid RecipientId { get; private set; }
        public UserType RecipientType { get; private set; }
        public NotificationType Type { get; private set; }
        public NotificationChannel Channel { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public NotificationPriority Priority { get; private set; }
        public NotificationStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ScheduledAt { get; private set; }
        public DateTime? SentAt { get; private set; }
        public int RetryCount { get; private set; }
        public string? ErrorMessage { get; private set; }
        protected Notification() { }

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
