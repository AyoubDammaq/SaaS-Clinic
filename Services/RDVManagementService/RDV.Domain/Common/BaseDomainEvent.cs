using MediatR;

namespace RDV.Domain.Common
{
    public abstract class BaseDomainEvent : INotification
    {
        public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    }
}
