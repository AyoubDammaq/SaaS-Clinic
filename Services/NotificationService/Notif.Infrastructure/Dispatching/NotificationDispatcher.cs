using Microsoft.Extensions.Logging;
using Notif.Domain.Entities;
using Notif.Domain.Enums;
using Notif.Domain.Interfaces;
using Notif.Domain.ValueObject;

namespace Notif.Infrastructure.Dispatching
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly INotificationChannelFactory _channelFactory;
        private readonly ILogger<NotificationDispatcher> _logger;
        private readonly INotificationRepository _notificationRepository;


        public NotificationDispatcher(INotificationChannelFactory channelFactory, ILogger<NotificationDispatcher> logger, INotificationRepository notificationRepository)
        {
            _channelFactory = channelFactory;
            _logger = logger;
            _notificationRepository = notificationRepository;
        }

        public async Task DispatchAsync(Notification notification)
        {
            // ✅ Vérification si déjà envoyé
            var existing = await _notificationRepository.GetByIdAsync(notification.Id);
            if (existing != null && existing.Status == NotificationStatus.Sent)
            {
                _logger.LogInformation("Notification {NotificationId} déjà envoyée, ignorée.", notification.Id);
                return;
            }

            var channelsToSend = new[]
            {
                NotificationChannel.Email,
            };

            bool atLeastOneSuccess = false;

            foreach (var channelType in channelsToSend)
            {
                var channel = _channelFactory.CreateChannel(channelType);

                var message = new NotificationMessage
                {
                    RecipientId = notification.RecipientId,
                    RecipientType = notification.RecipientType,
                    Title = notification.Title,
                    Content = notification.Content,
                    Channel = channelType
                };

                try
                {
                    var success = await channel.SendAsync(message);
                    if (success)
                    {
                        _logger.LogInformation("Notification {NotificationId} sent via {Channel}", notification.Id, channelType);
                        atLeastOneSuccess = true;
                    }
                    else
                    {
                        _logger.LogWarning("Failed to send notification {NotificationId} via {Channel}", notification.Id, channelType);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception while sending notification {NotificationId} via {Channel}", notification.Id, channelType);
                }
            }

            if (atLeastOneSuccess)
            {
                notification.MarkAsSent();
            }
            else
            {
                notification.MarkAsFailed("All channels failed to send.");
            }
        }
    }
}
