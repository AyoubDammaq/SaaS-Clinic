using Microsoft.EntityFrameworkCore;
using Notif.Domain.Entities;
using Notif.Domain.Enums;
using Notif.Domain.Interfaces;
using Notif.Infrastructure.Data;

namespace Notif.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetAllAsync(int skip = 0, int take = 50)
        {
            return await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task<List<Notification>> GetPendingNotificationsAsync()
        {
            return await _context.Notifications
                .Where(n => n.Status == NotificationStatus.Pending)
                .OrderBy(n => n.Priority)
                .ThenBy(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetFailedNotificationsForRetryAsync()
        {
            return await _context.Notifications
                .Where(n => n.Status == NotificationStatus.Failed && n.RetryCount < 3)
                .ToListAsync();
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetByRecipientAsync(Guid recipientId, int skip = 0, int take = 50)
        {
            return await _context.Notifications
                .Where(n => n.RecipientId == recipientId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Notification>> FilterAsync(
            Guid? recipientId = null,
            UserType? recipientType = null,
            NotificationStatus? status = null,
            NotificationType? type = null,
            DateTime? from = null,
            DateTime? to = null,
            int skip = 0,
            int take = 50)
        {
            var query = _context.Notifications.AsQueryable();

            if (recipientId.HasValue)
                query = query.Where(n => n.RecipientId == recipientId.Value);

            if (recipientType.HasValue)
                query = query.Where(n => n.RecipientType == recipientType.Value);

            if (status.HasValue)
                query = query.Where(n => n.Status == status.Value);

            if (type.HasValue)
                query = query.Where(n => n.Type == type.Value);

            if (from.HasValue)
                query = query.Where(n => n.CreatedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(n => n.CreatedAt <= to.Value);

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetNotificationsByRecipientIdAsync(Guid RecipientId)
        {
            return await _context.Notifications
                .Where(n => n.RecipientId == RecipientId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteNotificationAsync(Guid NotificationId)
        {
            var notification = await _context.Notifications.FindAsync(NotificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllNotificationsAsync(Guid RecipientId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == RecipientId)
                .ToListAsync();
            if (notifications.Any())
            {
                _context.Notifications.RemoveRange(notifications);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkNotificationAsReadAsync(Notification notification)
        {
            var entity = await _context.Notifications.FindAsync(notification.Id);
            if (entity == null) return;

            if (entity.Status != NotificationStatus.IsRead)
            {
                entity.Status = NotificationStatus.IsRead;
                _context.Notifications.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllNotificationsAsReadAsync(Guid RecipientId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == RecipientId && n.Status != NotificationStatus.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.Status = NotificationStatus.IsRead;
            }

            await _context.SaveChangesAsync();
        }
    }
}
