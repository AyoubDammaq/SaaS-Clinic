using MediatR;

namespace Clinic.Domain.Common
{
    public abstract class BaseDomainEvent : INotification
    {
        public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    }
}
