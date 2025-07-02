using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notif.Domain.Channels;
using Notif.Domain.Enums;
using Notif.Domain.ValueObject;

namespace Notif.Infrastructure.Channels
{
    public class PushChannel : INotificationChannel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PushChannel> _logger;

        public NotificationChannel ChannelType => NotificationChannel.Push;

        public PushChannel(IConfiguration configuration, ILogger<PushChannel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public Task<bool> SendAsync(NotificationMessage message)
        {
            // TODO: Intégration avec Firebase Cloud Messaging ou autre service Push
            Console.WriteLine($"[Push] Sending to {message.RecipientId}: {message.Title} - {message.Content}");
            return Task.FromResult( false );
        }
    }

}
