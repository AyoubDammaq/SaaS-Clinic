using Microsoft.Extensions.Logging;
using Notif.Application.DTOs;
using Notif.Application.Interfaces;
using Notif.Domain.Entities;
using Notif.Domain.Interfaces;

namespace Notif.Application.Services
{
    public class NotificationApplicationService : INotificationApplicationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationApplicationService> _logger;

        public NotificationApplicationService(
            INotificationRepository notificationRepository,
            ILogger<NotificationApplicationService> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification is null) return null;

            return new NotificationDto(
                Id: notification.Id,
                RecipientId: notification.RecipientId,
                RecipientType: notification.RecipientType,
                Type: notification.Type,
                Channel: notification.Channel,
                Title: notification.Title,
                Content: notification.Content,
                Priority: notification.Priority,
                Status: notification.Status,
                CreatedAt: notification.CreatedAt,
                SentAt: notification.SentAt
            );
        }


        public async Task<IEnumerable<NotificationSummaryDto>> GetNotificationsAsync(NotificationFilterRequest filter)
        {
            // Calcul du skip et take à partir de Page et PageSize
            var skip = (filter.Page - 1) * filter.PageSize;
            var take = filter.PageSize;

            IEnumerable<Notification> notifications;

            if (filter.RecipientId.HasValue)
            {
                // Notifications spécifiques à un destinataire
                notifications = await _notificationRepository.GetByRecipientAsync(filter.RecipientId.Value, skip, take);
            }
            else
            {
                // Cas SuperAdmin : récupérer toutes les notifications
                notifications = await _notificationRepository.GetAllAsync(skip, take);
            }

            // Projection vers le DTO
            return notifications.Select(n => new NotificationSummaryDto(
                Id: n.Id,
                Title: n.Title,
                Status: n.Status,
                CreatedAt: n.CreatedAt,
                SentAt: n.SentAt
            )).ToList();
        }

        public async Task MarkAsSentAsync(MarkNotificationAsSentRequest request)
        {
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId);
            if (notification is null)
                throw new Exception($"Notification {request.NotificationId} introuvable.");

            notification.MarkAsSent();
            await _notificationRepository.UpdateAsync(notification);
        }
     
        public async Task<List<NotificationDto>> GetNotificationsByRecipientId(Guid recipientId)
        {
            var notifications = await _notificationRepository.GetNotificationsByRecipientIdAsync(recipientId);
            return notifications.Select(n => new NotificationDto(
                Id: n.Id,
                RecipientId: n.RecipientId,
                RecipientType: n.RecipientType,
                Type: n.Type,
                Channel: n.Channel,
                Title: n.Title,
                Content: n.Content,
                Priority: n.Priority,
                Status: n.Status,
                CreatedAt: n.CreatedAt,
                SentAt: n.SentAt
            )).ToList();
        }

        public async Task DeleteNotification(Guid notificationId)
        {
            await _notificationRepository.DeleteNotificationAsync(notificationId);
        }


        public async Task DeleteAllNotifications(Guid recipientId)
        {
            await _notificationRepository.DeleteAllNotificationsAsync(recipientId);
        }

        public async Task MarkNotificationAsRead(NotificationDto notificationDto)
        {
            if (notificationDto == null)
                throw new ArgumentNullException(nameof(notificationDto));

            // Map NotificationDto to Notification
            var notification = new Notification
            {
                Id = notificationDto.Id,
                RecipientId = notificationDto.RecipientId,
                RecipientType = notificationDto.RecipientType,
                Type = notificationDto.Type,
                Channel = notificationDto.Channel,
                Title = notificationDto.Title,
                Content = notificationDto.Content,
                Priority = notificationDto.Priority,
                Status = notificationDto.Status,
                CreatedAt = notificationDto.CreatedAt,
                SentAt = notificationDto.SentAt,
                // Note: RetryCount, ErrorMessage, and ScheduledAt are not in NotificationDto
                RetryCount = 0, // Default or fetch from repository if needed
                ErrorMessage = null,
                ScheduledAt = null
            };

            await _notificationRepository.MarkNotificationAsReadAsync(notification);
        }

        public async Task MarkAllNotificationsAsRead(Guid recipientId)
        {
            await _notificationRepository.MarkAllNotificationsAsReadAsync(recipientId);
        }
    }

}