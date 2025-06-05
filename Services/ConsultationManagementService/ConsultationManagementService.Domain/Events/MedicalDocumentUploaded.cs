using ConsultationManagementService.Domain.Common;
using ConsultationManagementService.Domain.Entities;

namespace ConsultationManagementService.Domain.Events
{
    public class MedicalDocumentUploaded : BaseDomainEvent
    {
        public DocumentMedical DocumentMedical { get; }
        public MedicalDocumentUploaded(DocumentMedical documentMedical)
        {
            DocumentMedical = documentMedical;

        }
    }
}
