using Microsoft.Extensions.Logging;
using Notif.Application.DTOs;
using Notif.Application.Interfaces;
using Notif.Domain.Entities;
using Notif.Domain.Enums;
using Notif.Domain.Interfaces;

namespace Notif.Application.Services
{
    public class NotificationApplicationService : INotificationApplicationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationDispatcher _dispatcher;
        private readonly ILogger<NotificationApplicationService> _logger;

        public NotificationApplicationService(
            INotificationRepository notificationRepository,
            INotificationDispatcher dispatcher,
            ILogger<NotificationApplicationService> logger)
        {
            _notificationRepository = notificationRepository;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        public async Task<Guid> CreateNotificationAsync(CreateNotificationRequest command)
        {
            var notification = Notification.Create(
                recipientId: command.RecipientId,
                recipientType: command.RecipientType,
                type: command.Type,
                channel: NotificationChannel.None, // utilisé pour initialiser, les canaux seront traités dans le dispatcher
                title: command.Title,
                content: command.Content,
                priority: command.Priority,
                scheduledAt: command.ScheduledAt
            );

            await _notificationRepository.AddAsync(notification);

            try
            {
                await _dispatcher.DispatchAsync(notification); // envoie sur Email + InApp
                await _notificationRepository.UpdateAsync(notification); // mise à jour statut
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi de la notification {NotificationId}", notification.Id);
                notification.MarkAsFailed(ex.Message);
                await _notificationRepository.UpdateAsync(notification);
            }

            return notification.Id;
        }


        public async Task SendNotificationAsync(SendNotificationRequest request)
        {
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId);
            if (notification is null)
                throw new Exception($"Notification {request.NotificationId} introuvable.");

            if (notification.Status != NotificationStatus.Pending)
                return;

            await _dispatcher.DispatchAsync(notification);
            await _notificationRepository.UpdateAsync(notification);
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

            // Si RecipientId n'est pas spécifié, on ne peut pas appeler la méthode actuelle du repository
            if (filter.RecipientId == null)
                throw new ArgumentException("RecipientId is required for this filter.");

            // Appel du repository
            var notifications = await _notificationRepository.GetByRecipientAsync(filter.RecipientId.Value, skip, take);

            // Projection vers le DTO
            return notifications.Select(n => new NotificationSummaryDto(
                Id: n.Id,
                Title: n.Title,
                Status: n.Status,
                CreatedAt: n.CreatedAt,
                SentAt: n.SentAt
            )).ToList();
        }

        // Méthode de traitement des notifications en attente
        public async Task ProcessPendingNotificationsAsync()
        {
            var pending = await _notificationRepository.GetPendingNotificationsAsync();

            foreach (var notification in pending)
            {
                try
                {
                    await _dispatcher.DispatchAsync(notification);
                    await _notificationRepository.UpdateAsync(notification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de l'envoi de la notification {NotificationId}", notification.Id);
                    notification.MarkAsFailed(ex.Message);
                    await _notificationRepository.UpdateAsync(notification);
                }
            }
        }

        // Méthode de retry en cas d’échec
        public async Task RetryFailedNotificationsAsync()
        {
            var failed = await _notificationRepository.GetFailedNotificationsForRetryAsync();

            foreach (var notification in failed)
            {
                if (!notification.CanRetry()) continue;

                try
                {
                    notification.Retry();
                    await _dispatcher.DispatchAsync(notification);
                    await _notificationRepository.UpdateAsync(notification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors du retry de la notification {NotificationId}", notification.Id);
                    notification.MarkAsFailed(ex.Message);
                    await _notificationRepository.UpdateAsync(notification);
                }
            }
        }

        public async Task MarkAsSentAsync(MarkNotificationAsSentRequest request)
        {
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId);
            if (notification is null)
                throw new Exception($"Notification {request.NotificationId} introuvable.");

            notification.MarkAsSent();
            await _notificationRepository.UpdateAsync(notification);
        }


        /*
        public async Task<NotificationPreferenceDto?> GetUserPreferencesAsync(Guid userId, UserType userType)
        {
            var preferences = await _userPreferenceRepository.GetByUserIdAsync(userId);
            if (preferences == null || preferences.UserType != userType)
                return null;

            return preferences.ToDto();
        }


        public async Task UpdateUserPreferencesAsync(UpdateNotificationPreferenceRequest request)
        {
            var preferences = await _userPreferenceRepository.GetByUserIdAsync(request.UserId);

            if (preferences == null)
            {
                // Créer une nouvelle préférence
                preferences = new UserNotificationPreferences
                {
                    UserId = request.UserId,
                    UserType = request.UserType,
                    PreferredChannels = request.PreferredChannels,
                    NotificationSettings = request.NotificationSettings,
                    Language = request.Language,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email
                };
            }
            else
            {
                // Mettre à jour la préférence existante
                preferences.UserType = request.UserType;
                preferences.PreferredChannels = request.PreferredChannels;
                preferences.NotificationSettings = request.NotificationSettings;
                preferences.Language = request.Language;
                preferences.PhoneNumber = request.PhoneNumber;
                preferences.Email = request.Email;
            }

            await _userPreferenceRepository.SaveAsync(preferences);
        }

        */
    }

}