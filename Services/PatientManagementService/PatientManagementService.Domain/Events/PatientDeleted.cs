using PatientManagementService.Domain.Common;

namespace PatientManagementService.Domain.Events
{
    public class PatientDeleted : BaseDomainEvent
    {
        public Guid PatientId { get; }

        public PatientDeleted(Guid patientId)
        {
            PatientId = patientId;
        }
    }
}
