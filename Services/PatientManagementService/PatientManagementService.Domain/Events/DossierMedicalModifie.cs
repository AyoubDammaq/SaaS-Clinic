using MediatR;
using PatientManagementService.Domain.Common;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Domain.Events
{
    public class DossierMedicalModifie : BaseDomainEvent
    {
        public DossierMedical DossierMedical { get; }

        public DossierMedicalModifie(DossierMedical dossierMedical)
        {
            DossierMedical = dossierMedical;
        }
    }
}
