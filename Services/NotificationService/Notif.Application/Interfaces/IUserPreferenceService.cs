using Notif.Domain.Entities;
using Notif.Domain.Enums;
using Notif.Domain.ValueObject;

namespace Notif.Application.Interfaces
{
    public interface IUserPreferenceService
    {
        Task<UserNotificationPreferences> GetPreferencesForUserAsync(Guid userId, UserType userType);
    }
}
