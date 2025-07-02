using Notif.Domain.Enums;
using Notif.Domain.ValueObject;

namespace Notif.Domain.Channels
{
    public interface INotificationChannel
    {
        NotificationChannel ChannelType { get; }
        Task<bool> SendAsync(NotificationMessage message);
    }
}
