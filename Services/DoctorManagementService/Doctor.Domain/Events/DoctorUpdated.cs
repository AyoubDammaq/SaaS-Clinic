using Doctor.Domain.Entities;
using Doctor.Domain.Events.Clinic.Domain.Common;

namespace Doctor.Domain.Events
{
    public class DoctorUpdated : BaseDomainEvent
    {
        public Medecin Medecin { get; }

        public DoctorUpdated(Medecin medecin)
        {
            Medecin = medecin;
        }
    }
}
