using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notif.Domain.Channels;
using Notif.Domain.Enums;
using Notif.Domain.ValueObject;

namespace Notif.Infrastructure.Channels
{
    public class InAppChannel : INotificationChannel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<InAppChannel> _logger;

        public NotificationChannel ChannelType => NotificationChannel.InApp;

        public InAppChannel(IConfiguration configuration, ILogger<InAppChannel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public Task<bool> SendAsync(NotificationMessage message)
        {
            // TODO: Implémenter la logique d'affichage dans l'application (ex: notification WebSocket)
            _logger.LogWarning("⚠️ Envoi InApp non implémenté. Notification pour {RecipientId} ignorée.", message.RecipientId);
            return Task.FromResult( false );
        }
    }
}
