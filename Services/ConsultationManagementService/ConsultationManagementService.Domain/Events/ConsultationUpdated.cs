using ConsultationManagementService.Domain.Common;
using ConsultationManagementService.Domain.Entities;

namespace ConsultationManagementService.Domain.Events
{
    public class ConsultationUpdated : BaseDomainEvent
    {
        public Consultation Consultation { get; }
        public ConsultationUpdated(Consultation consultation)
        {
            Consultation = consultation;
        }
    }
}
