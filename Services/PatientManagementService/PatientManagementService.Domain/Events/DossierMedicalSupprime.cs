using PatientManagementService.Domain.Common;

namespace PatientManagementService.Domain.Events
{
    public class DossierMedicalSupprime : BaseDomainEvent
    {
        public Guid DossierMedicalId { get; }

        public DossierMedicalSupprime(Guid dossierMedicalId)
        {
            DossierMedicalId = dossierMedicalId;
        }
    }
}
