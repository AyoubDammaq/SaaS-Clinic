using Notification.Domain.Entities;
using Notification.Domain.Enums;

namespace Notification.Domain.Interfaces
{
    public interface INotificationRepository
    {
        Task<Domain.Entities.Notification> AddAsync(Domain.Entities.Notification notif);
        Task<IEnumerable<Domain.Entities.Notification>> GetByUserAsync(Guid userId, StatutNotification? statut = null, TypeNotification? type = null);
        Task<Domain.Entities.Notification?> GetByIdAsync(Guid id);
        Task<bool> MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync(Guid userId);
        Task<bool> DeleteAsync(Guid id);
        Task<PreferenceNotification?> GetPreferencesAsync(Guid userId);
        Task SetPreferencesAsync(PreferenceNotification preferences);
    }
}
