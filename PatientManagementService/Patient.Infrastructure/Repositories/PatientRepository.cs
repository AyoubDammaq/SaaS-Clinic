﻿using Microsoft.EntityFrameworkCore;
using PatientManagementService.Data;
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
            return await _context.Patients
                .Include(p => p.DossierMedical)
                    .ThenInclude(d => d.Documents)
                .ToListAsync();
        }


        public async Task<Patient?> GetPatientByIdAsync(Guid id)
        {
            return await _context.Patients
                .Include(p => p.DossierMedical)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddPatientAsync(Patient patient)
        {  
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
            var patientUpdated = await _context.Patients.FindAsync(patient.Id);
            if (patientUpdated != null)
            {
                _context.Patients.Update(patientUpdated);
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

        public async Task<IEnumerable<Patient>> GetPatientsByNameAsync(string? name, string? lastname)
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
