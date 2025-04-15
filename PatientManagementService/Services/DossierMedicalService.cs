using PatientManagementService.DTOs;
using PatientManagementService.Models;
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

        public async Task<DossierMedical> GetDossierMedicalByPatientIdAsync(Guid patientId)
        {
            return await _dossierMedicalRepository.GetDossierMedicalByPatientIdAsync(patientId);
        }

        public async Task AddDossierMedicalAsync(DossierMedicalDTO dossierMedical)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(dossierMedical.PatientId);
            if (patient == null)
            {
                throw new Exception("Patient not found");
            }
            patient.DossierMedicalId = dossierMedical.Id;
            await _dossierMedicalRepository.AddDossierMedicalAsync(dossierMedical);
        }

        public async Task UpdateDossierMedicalAsync(DossierMedicalDTO dossierMedical)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(dossierMedical.PatientId);
            if (patient == null)
            {
                throw new Exception("Patient not found");
            }
            await _dossierMedicalRepository.UpdateDossierMedicalAsync(dossierMedical);
        }

        public async Task DeleteDossierMedicalAsync(Guid dossierMedicalId)
        {
            var dossierMedical = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(dossierMedicalId);
            if (dossierMedical == null)
            {
                throw new Exception("Dossier médical not found");
            }
            await _dossierMedicalRepository.DeleteDossierMedicalAsync(dossierMedicalId);
        }

        public async Task<IEnumerable<DossierMedical>> GetAllDossiersMedicalsAsync()
        {
            return await _dossierMedicalRepository.GetAllDossiersMedicalsAsync();
        }

        public async Task<DossierMedical> GetDossierMedicalByIdAsync(Guid Id)
        {
            var dossierMedical = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(Id);
            if (dossierMedical == null)
            {
                throw new Exception("Dossier médical not found");
            }
            return dossierMedical;
        }

        public async Task AttacherDocumentAsync(Guid dossierMedicalId, Document document)
        {
            var dossierMedical = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(dossierMedicalId);
            if (dossierMedical == null)
            {
                throw new InvalidOperationException("Dossier médical not found.");
            }
            await _dossierMedicalRepository.AttacherDocumentAsync(dossierMedicalId, document);
        }
    }
}
