using Notif.Application.DTOs;
using Notif.Domain.Entities;

namespace Notif.Application.Interfaces
{
    public interface INotificationApplicationService
    {
        Task<Guid> CreateNotificationAsync(CreateNotificationRequest request);
        Task SendNotificationAsync(SendNotificationRequest request);
        Task<NotificationDto?> GetNotificationByIdAsync(Guid id);
        Task<IEnumerable<NotificationSummaryDto>> GetNotificationsAsync(NotificationFilterRequest filter);
        Task MarkAsSentAsync(MarkNotificationAsSentRequest request);
        //Task<NotificationPreferenceDto?> GetUserPreferencesAsync(Guid userId, UserType userType);
        //Task UpdateUserPreferencesAsync(UpdateNotificationPreferenceRequest request);

        Task<List<NotificationDto>> GetNotificationsByRecipientId(Guid recipientId);
        Task DeleteNotification(Guid notificationId);
        Task MarkNotificationAsRead(NotificationDto notificationDto);
        Task MarkAllNotificationsAsRead(Guid recipientId);
    }
}
