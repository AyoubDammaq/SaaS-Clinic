using PatientManagementService.Domain.Common;

namespace PatientManagementService.Domain.Events
{
    public class PatientAdded : BaseDomainEvent
    {
        public Guid PatientId { get; }
        public string Nom { get; }
        public string Prenom { get; }

        public PatientAdded(Guid patientId, string nom, string prenom)
        {
            PatientId = patientId;
            Nom = nom;
            Prenom = prenom;
        }
    }
}
