using PatientManagementService.Repositories;

namespace PatientManagementService.Services
{
    public class DossierMedicalService : IDossierMedicalService
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;
        private readonly IPatientRepository _patientRepository;
        public DossierMedicalService(IDossierMedicalRepository dossierMedicalRepository, IPatientRepository patientRepository)
        {
            _dossierMedicalRepository = dossierMedicalRepository;
            _patientRepository = patientRepository;
        }
    }
}
