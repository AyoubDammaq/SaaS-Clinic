using Microsoft.EntityFrameworkCore;
using Notif.Domain.Interfaces;
using Notif.Domain.ValueObject;
using Notif.Infrastructure.Data;

namespace Notif.Infrastructure.Repositories
{
    public class UserPreferenceRepository : IUserPreferenceRepository
    {
        private readonly NotificationDbContext _dbContext;

        public UserPreferenceRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<UserNotificationPreferences?> GetByUserIdAsync(Guid userId)
        {
            return _dbContext.UserPreferences
                             .Where(p => p.UserId == userId)
                             .FirstOrDefaultAsync();
        }

        public async Task SaveAsync(UserNotificationPreferences preferences)
        {
            var existing = await _dbContext.UserPreferences
                .FirstOrDefaultAsync(p => p.UserId == preferences.UserId);

            if (existing == null)
            {
                // Nouvelle entrée
                await _dbContext.UserPreferences.AddAsync(preferences);
            }
            else
            {
                // Mise à jour des champs existants
                existing.UserType = preferences.UserType;
                existing.PreferredChannels = preferences.PreferredChannels;
                existing.NotificationSettings = preferences.NotificationSettings;
                existing.Language = preferences.Language;
                existing.PhoneNumber = preferences.PhoneNumber;
                existing.Email = preferences.Email;

                _dbContext.UserPreferences.Update(existing);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
