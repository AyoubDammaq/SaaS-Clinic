using PatientManagementService.Domain.Common;
using PatientManagementService.Domain.Entities;


namespace PatientManagementService.Domain.Events
{
    public class DocumentAttacheAuDossier : BaseDomainEvent
    {
        public Document Document { get; }
        public Guid DossierMedicalId { get; }

        public DocumentAttacheAuDossier(Document document, Guid dossierMedicalId)
        {
            Document = document;
            DossierMedicalId = dossierMedicalId;
        }
    }
}
