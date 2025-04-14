using Microsoft.EntityFrameworkCore;
using PatientManagementService.Data;
using PatientManagementService.DTOs;
using PatientManagementService.Models;

namespace PatientManagementService.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientDbContext _context;
        public PatientRepository(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<Patient> GetPatientByIdAsync(Guid id)
        {
            return await _context.Patients.FindAsync(id);
        }

        public async Task AddPatientAsync(PatientDTO patientDto)
        {
            var patient = new Patient
            {
                Id = patientDto.Id,
                Nom = patientDto.Nom,
                Prenom = patientDto.Prenom,
                DateNaissance = patientDto.DateNaissance,
                Sexe = patientDto.Sexe,
                Adresse = patientDto.Adresse,
                NumeroTelephone = patientDto.NumeroTelephone,
                Email = patientDto.Email
            };

            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePatientAsync(PatientDTO patientDto)
        {
            var patient = await _context.Patients.FindAsync(patientDto.Id);
            if (patient != null)
            {
                patient.Nom = patientDto.Nom;
                patient.Prenom = patientDto.Prenom;
                patient.DateNaissance = patientDto.DateNaissance;
                patient.Sexe = patientDto.Sexe;
                patient.Adresse = patientDto.Adresse;
                patient.NumeroTelephone = patientDto.NumeroTelephone;
                patient.Email = patientDto.Email;

                _context.Patients.Update(patient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeletePatientAsync(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Patient>> GetPatientsByNameAsync(string name, string lastname)
        {
            name = name?.Trim().ToLower();
            lastname = lastname?.Trim().ToLower();

            return await _context.Patients
                .Where(m =>
                    (string.IsNullOrEmpty(name) || m.Nom.ToLower().Contains(name)) &&
                    (string.IsNullOrEmpty(lastname) || m.Prenom.ToLower().Contains(lastname))
                )
                .ToListAsync();
        }
    }
}
