using ConsultationManagementService.Domain.Common;
using ConsultationManagementService.Domain.Entities;

namespace ConsultationManagementService.Domain.Events
{
    public class MedicalDocumentRemoved : BaseDomainEvent
    {
        public DocumentMedical DocumentMedical { get; }
        public MedicalDocumentRemoved(DocumentMedical documentMedical)
        {
            DocumentMedical = documentMedical;
        }
    }
}
