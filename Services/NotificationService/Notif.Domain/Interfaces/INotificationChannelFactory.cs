using Notif.Domain.Channels;
using Notif.Domain.Enums;

namespace Notif.Domain.Interfaces
{
    public interface INotificationChannelFactory
    {
        INotificationChannel CreateChannel(NotificationChannel channelType);
    }
}
