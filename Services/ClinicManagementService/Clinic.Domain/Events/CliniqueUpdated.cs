using Clinic.Domain.Common;
using Clinic.Domain.Entities;

namespace Clinic.Domain.Events
{
    public class CliniqueUpdated : BaseDomainEvent
    {
        public Clinique Clinique { get; }

        public CliniqueUpdated(Clinique clinique)
        {
            Clinique = clinique;
        }
    }
}
