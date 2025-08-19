using Notif.Domain.Entities;
using Notif.Domain.Enums;

namespace Notif.Domain.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetAllAsync(int skip = 0, int take = 50);
        Task<Notification?> GetByIdAsync(Guid id);
        Task<List<Notification>> GetPendingNotificationsAsync();
        Task<List<Notification>> GetFailedNotificationsForRetryAsync();
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task<List<Notification>> GetByRecipientAsync(Guid recipientId, int skip = 0, int take = 50);
        Task<List<Notification>> FilterAsync(Guid? recipientId = null, UserType? recipientType = null, NotificationStatus? status = null, NotificationType? type = null,
            DateTime? from = null, DateTime? to = null, int skip = 0, int take = 50);

        Task<List<Notification>> GetNotificationsByRecipientIdAsync(Guid RecipientId);
        Task DeleteNotificationAsync(Guid NotificationId);
        Task DeleteAllNotificationsAsync(Guid RecipientId);
        Task MarkNotificationAsReadAsync(Notification notification);
        Task MarkAllNotificationsAsReadAsync(Guid RecipientId);
    }
}
