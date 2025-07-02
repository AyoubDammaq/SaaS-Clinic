using Notif.Application.Interfaces;
using Notif.Domain.Enums;
using Notif.Domain.Interfaces;
using Notif.Domain.ValueObject;

namespace Notif.Application.Services
{
    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly IUserPreferenceRepository _repository;

        public UserPreferenceService(IUserPreferenceRepository repository)
        {
            _repository = repository;
        }

        public Task<UserNotificationPreferences> GetPreferencesForUserAsync(Guid userId, UserType userType)
        {
            return _repository.GetByUserIdAsync(userId);
        }
    }

}
