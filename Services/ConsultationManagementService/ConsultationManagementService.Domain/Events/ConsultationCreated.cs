using ConsultationManagementService.Domain.Common;
using ConsultationManagementService.Domain.Entities;

namespace ConsultationManagementService.Domain.Events
{
    public class ConsultationCreated : BaseDomainEvent
    {
        public Consultation Consultation { get; }
        public ConsultationCreated(Consultation consultation) 
        {
            Consultation = consultation;
        }
    }
}
