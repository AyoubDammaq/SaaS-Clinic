using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities;
using Notification.Domain.Enums;
using Notification.Domain.Interfaces;
using Notification.Infrastructure.Data;

namespace Notification.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Notification> AddAsync(Domain.Entities.Notification notif)
        {
            _context.Notifications.Add(notif);
            await _context.SaveChangesAsync();
            return notif;
        }

        public async Task<IEnumerable<Domain.Entities.Notification>> GetByUserAsync(Guid userId, StatutNotification? statut = null, TypeNotification? type = null)
        {
            var query = _context.Notifications.AsQueryable()
                .Where(n => n.UtilisateurId == userId);

            if (statut.HasValue)
                query = query.Where(n => n.Statut == statut.Value);

            if (type.HasValue)
                query = query.Where(n => n.Type == type.Value);

            return await query.OrderByDescending(n => n.DateEnvoi).ToListAsync();
        }

        public async Task<Domain.Entities.Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task<bool> MarkAsReadAsync(Guid id)
        {
            var notif = await _context.Notifications.FindAsync(id);
            if (notif == null) return false;

            notif.Statut = StatutNotification.LU;
            notif.DateLecture = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var notifs = await _context.Notifications
                .Where(n => n.UtilisateurId == userId && n.Statut == StatutNotification.NON_LU)
                .ToListAsync();

            foreach (var n in notifs)
            {
                n.Statut = StatutNotification.LU;
                n.DateLecture = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var notif = await _context.Notifications.FindAsync(id);
            if (notif == null) return false;

            _context.Notifications.Remove(notif);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PreferenceNotification?> GetPreferencesAsync(Guid userId)
        {
            return await _context.Preferences.FindAsync(userId);
        }

        public async Task SetPreferencesAsync(PreferenceNotification preferences)
        {
            var existing = await _context.Preferences.FindAsync(preferences.UtilisateurId);
            if (existing == null)
            {
                _context.Preferences.Add(preferences);
            }
            else
            {
                _context.Entry(existing).CurrentValues.SetValues(preferences);
            }
            await _context.SaveChangesAsync();
        }
    }
}
