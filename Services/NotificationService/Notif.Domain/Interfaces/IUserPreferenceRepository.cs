using Notif.Domain.ValueObject;

namespace Notif.Domain.Interfaces
{
    public interface IUserPreferenceRepository
    {
        Task<UserNotificationPreferences?> GetByUserIdAsync(Guid userId);
        Task SaveAsync(UserNotificationPreferences preferences);
    }

}
