using MediatR;

namespace Doctor.Domain.Events
{
    namespace Clinic.Domain.Common
    {
        public abstract class BaseDomainEvent : INotification
        {
            public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
        }
    }
}
