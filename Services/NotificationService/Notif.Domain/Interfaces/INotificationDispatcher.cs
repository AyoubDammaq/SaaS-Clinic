using Notif.Domain.Entities;
using Notif.Domain.ValueObject;

namespace Notif.Domain.Interfaces
{
    public interface INotificationDispatcher
    {
        Task DispatchAsync(Notification notification);
    }
}
