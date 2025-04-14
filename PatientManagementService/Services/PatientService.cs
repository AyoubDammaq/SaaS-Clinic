using PatientManagementService.Repositories;

namespace PatientManagementService.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IDossierMedicalRepository _dossierMedicalRepository;
        public PatientService(IPatientRepository patientRepository, IDossierMedicalRepository dossierMedicalRepository)
        {
            _patientRepository = patientRepository;
            _dossierMedicalRepository = dossierMedicalRepository;
        }
    }
}
