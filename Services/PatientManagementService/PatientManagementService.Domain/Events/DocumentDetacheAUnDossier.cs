using PatientManagementService.Domain.Common;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Domain.Events
{
    public class DocumentDetacheAUnDossier : BaseDomainEvent
    {
        public Document Document { get; }
        public Guid DossierMedicalId { get; }

        public DocumentDetacheAUnDossier(Document document, Guid dossierMedicalId)
        {
            Document = document;
            DossierMedicalId = dossierMedicalId;
        }
    }
}
