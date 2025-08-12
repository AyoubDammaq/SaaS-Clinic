using Notif.Application.DTOs;

namespace Notif.Application.Interfaces
{
    public interface INotificationApplicationService
    {
        Task<NotificationDto?> GetNotificationByIdAsync(Guid id);
        Task<IEnumerable<NotificationSummaryDto>> GetNotificationsAsync(NotificationFilterRequest filter);
        Task MarkAsSentAsync(MarkNotificationAsSentRequest request);
        Task<List<NotificationDto>> GetNotificationsByRecipientId(Guid recipientId);
        Task DeleteNotification(Guid notificationId);
        Task DeleteAllNotifications(Guid recipientId);
        Task MarkNotificationAsRead(NotificationDto notificationDto);
        Task MarkAllNotificationsAsRead(Guid recipientId);
    }
}
