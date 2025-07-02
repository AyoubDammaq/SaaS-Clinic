using Microsoft.Extensions.DependencyInjection;
using Notif.Domain.Channels;
using Notif.Domain.Enums;
using Notif.Domain.Interfaces;
using Notif.Infrastructure.Channels;

namespace Notif.Infrastructure.Factories
{
    public class NotificationChannelFactory : INotificationChannelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationChannelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public INotificationChannel CreateChannel(NotificationChannel channelType)
        {
            return channelType switch
            {
                NotificationChannel.SMS => _serviceProvider.GetRequiredService<SmsChannel>(),
                NotificationChannel.Email => _serviceProvider.GetRequiredService<EmailChannel>(),
                NotificationChannel.Push => _serviceProvider.GetRequiredService<PushChannel>(),
                NotificationChannel.InApp => _serviceProvider.GetRequiredService<InAppChannel>(),
                _ => throw new ArgumentException($"Unknown channel type: {channelType}")
            };
        }
    }
}
