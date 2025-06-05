using ConsultationManagementService.Domain.Common;
using ConsultationManagementService.Domain.Entities;

namespace ConsultationManagementService.Domain.Events
{
    public class ConsultationDeleted : BaseDomainEvent
    {
        public Consultation Consultation { get; }

        public ConsultationDeleted(Consultation consultation)
        {
            Consultation = consultation;

        }
    }
}
