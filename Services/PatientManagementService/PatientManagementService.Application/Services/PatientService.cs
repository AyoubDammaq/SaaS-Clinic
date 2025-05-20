using PatientManagementService.Application.DTOs;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace PatientManagementService.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _patientRepository.GetAllPatientsAsync() ?? new List<Patient>();
        }

        public async Task<Patient?> GetPatientByIdAsync(Guid id)
        {
            return await _patientRepository.GetPatientByIdAsync(id);
        }

        public async Task<bool> AddPatientAsync(PatientDTO patient)
        {
            if (patient == null)
            {
                _logger.LogWarning("Tentative d'ajouter un patient null.");
                return false;
            }

            var newPatient = new Patient
            {
                Id = Guid.NewGuid(),
                Nom = patient.Nom,
                Prenom = patient.Prenom,
                DateNaissance = patient.DateNaissance,
                Sexe = patient.Sexe,
                Adresse = patient.Adresse,
                Telephone = patient.Telephone,
                Email = patient.Email
            };

            await _patientRepository.AddPatientAsync(newPatient);
            return true;
        }

        public async Task<bool> UpdatePatientAsync(PatientDTO patient)
        {
            if (patient == null)
            {
                _logger.LogWarning("Mise à jour échouée : patient null.");
                return false;
            }

            var existingPatient = await _patientRepository.GetPatientByIdAsync(patient.Id);
            if (existingPatient == null)
            {
                _logger.LogWarning($"Patient introuvable pour mise à jour (ID : {patient.Id}).");
                return false;
            }

            existingPatient.Nom = patient.Nom;
            existingPatient.Prenom = patient.Prenom;
            existingPatient.DateNaissance = patient.DateNaissance;
            existingPatient.Sexe = patient.Sexe;
            existingPatient.Adresse = patient.Adresse;
            existingPatient.Telephone = patient.Telephone;
            existingPatient.Email = patient.Email;

            await _patientRepository.UpdatePatientAsync(existingPatient);
            return true;
        }

        public async Task<bool> DeletePatientAsync(Guid id)
        {
            var existingPatient = await _patientRepository.GetPatientByIdAsync(id);
            if (existingPatient == null)
            {
                _logger.LogWarning($"Patient introuvable pour suppression (ID : {id}).");
                return false;
            }

            await _patientRepository.DeletePatientAsync(id);
            return true;
        }

        public async Task<IEnumerable<Patient>> GetPatientsByNameAsync(string? name, string? lastname)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(lastname))
            {
                _logger.LogWarning("Requête invalide : nom ou prénom manquant.");
                return Enumerable.Empty<Patient>();
            }

            return await _patientRepository.GetPatientsByNameAsync(name, lastname) ?? Enumerable.Empty<Patient>();
        }

        public async Task<int> GetStatistiquesAsync(DateTime dateDebut, DateTime dateFin)
        {
            return await _patientRepository.GetStatistiquesAsync(dateDebut, dateFin);
        }
    }
}
