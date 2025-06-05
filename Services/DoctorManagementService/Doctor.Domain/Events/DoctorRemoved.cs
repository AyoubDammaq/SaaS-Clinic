using Doctor.Domain.Entities;
using Doctor.Domain.Events.Clinic.Domain.Common;

namespace Doctor.Domain.Events
{
    public class DoctorRemoved : BaseDomainEvent
    {
        public Medecin Medecin { get; }

        public DoctorRemoved(Medecin medecin)
        {
            Medecin = medecin;
        }
    }
}
