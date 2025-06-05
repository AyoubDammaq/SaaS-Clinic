using PatientManagementService.Domain.Common;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Domain.Events
{
    public class DossierMedicalCree : BaseDomainEvent
    {
        public DossierMedical DossierMedical { get; }

        public DossierMedicalCree(DossierMedical dossierMedical)
        {
            DossierMedical = dossierMedical;
        }
    }
}
