using Clinic.Domain.Common;
using Clinic.Domain.Entities;

namespace Clinic.Domain.Events
{
    public class CliniqueDeleted : BaseDomainEvent
    {
        public Clinique Clinique { get; }

        public CliniqueDeleted(Clinique clinique)
        {
            Clinique = clinique;
        }
    }
}
