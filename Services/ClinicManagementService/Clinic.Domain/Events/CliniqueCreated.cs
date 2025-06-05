using Clinic.Domain.Common;
using Clinic.Domain.Entities;

namespace Clinic.Domain.Events
{
    public class CliniqueCreated : BaseDomainEvent
    {
        public Clinique Clinique { get; }

        public CliniqueCreated(Clinique clinique)
        {
            Clinique = clinique;
        }
    }
}
