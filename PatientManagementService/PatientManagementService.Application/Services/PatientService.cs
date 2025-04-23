using PatientManagementService.Application.DTOs;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllPatientsAsync();
            if (patients == null || !patients.Any())
            {
                throw new Exception("Aucun patient trouvé.");
            }
            return patients;
        }

        public async Task<Patient> GetPatientByIdAsync(Guid id)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(id) ?? throw new Exception($"Aucun patient trouvé avec l'ID {id}.");
            return patient;
        }

        public async Task AddPatientAsync(PatientDTO patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient), "Les informations du patient ne peuvent pas être nulles.");
            }

            var newPatient = new Patient
            {
                Id = Guid.NewGuid(),
                Nom = patient.Nom,
                Prenom = patient.Prenom,
                DateNaissance = patient.DateNaissance,
                Sexe = patient.Sexe,
                Adresse = patient.Adresse,
                NumeroTelephone = patient.NumeroTelephone,
                Email = patient.Email
            };

            await _patientRepository.AddPatientAsync(newPatient);
        }

        public async Task UpdatePatientAsync(PatientDTO patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient), "Les informations du patient ne peuvent pas être nulles.");
            }

            var existingPatient = await _patientRepository.GetPatientByIdAsync(patient.Id)
                ?? throw new Exception($"Aucun patient trouvé avec l'ID {patient.Id} pour mise à jour.");

            existingPatient.Nom = patient.Nom;
            existingPatient.Prenom = patient.Prenom;
            existingPatient.DateNaissance = patient.DateNaissance;
            existingPatient.Sexe = patient.Sexe;
            existingPatient.Adresse = patient.Adresse;
            existingPatient.NumeroTelephone = patient.NumeroTelephone;
            existingPatient.Email = patient.Email;

            await _patientRepository.UpdatePatientAsync(existingPatient);
        }

        public async Task DeletePatientAsync(Guid id)
        {
            var existingPatient = await _patientRepository.GetPatientByIdAsync(id) ?? throw new Exception($"Aucun patient trouvé avec l'ID {id} pour suppression.");
            await _patientRepository.DeletePatientAsync(id);
        }

        public async Task<IEnumerable<Patient>> GetPatientsByNameAsync(string? name, string? lastname)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(lastname))
            {
                throw new ArgumentException("Le nom et le prénom doivent être fournis.");
            }

            var patients = await _patientRepository.GetPatientsByNameAsync(name, lastname);
            if (patients == null || !patients.Any())
            {
                throw new Exception($"Aucun patient trouvé avec le nom {name} et le prénom {lastname}.");
            }

            return patients;
        }


        public async Task<int> GetStatistiquesAsync(DateTime dateDebut, DateTime dateFin)
        {
            return await _patientRepository.GetStatistiquesAsync(dateDebut, dateFin);
        }
    }
}
