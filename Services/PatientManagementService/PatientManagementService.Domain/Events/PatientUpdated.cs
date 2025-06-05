using PatientManagementService.Domain.Common;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Domain.Events
{
    public class PatientUpdated : BaseDomainEvent
    {
        public Patient Patient { get; }

        public PatientUpdated(Patient patient)
        {
            Patient = patient;
        }
    }
}
