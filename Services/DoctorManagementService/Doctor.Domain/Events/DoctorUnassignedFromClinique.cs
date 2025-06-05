using Doctor.Domain.Events.Clinic.Domain.Common;

namespace Doctor.Domain.Events
{
    public class DoctorUnassignedFromClinique : BaseDomainEvent
    {
        public Guid MedecinId { get; }
        public Guid CliniqueId { get; }

        public DoctorUnassignedFromClinique(Guid medecinId, Guid cliniqueId)
        {
            MedecinId = medecinId;
            CliniqueId = cliniqueId;
        }
    }

}
