using Doctor.Domain.Entities;
using Doctor.Domain.Events.Clinic.Domain.Common;

namespace Doctor.Domain.Events
{
    public class DoctorCreated : BaseDomainEvent
    {
        public Medecin Medecin { get; }

        public DoctorCreated(Medecin medecin)
        {
            Medecin = medecin;
        }
    }
}
