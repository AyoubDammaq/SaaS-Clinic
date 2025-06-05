using Doctor.Domain.Events.Clinic.Domain.Common;

namespace Doctor.Domain.Events
{
    public class DoctorAssignedToClinique : BaseDomainEvent
    {
        public Guid MedecinId { get; }
        public Guid CliniqueId { get; }

        public DoctorAssignedToClinique(Guid medecinId, Guid cliniqueId)
        {
            MedecinId = medecinId;
            CliniqueId = cliniqueId;
        }
    }
}
