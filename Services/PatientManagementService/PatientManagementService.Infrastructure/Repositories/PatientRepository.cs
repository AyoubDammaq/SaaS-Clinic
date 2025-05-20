using Microsoft.EntityFrameworkCore;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;
using PatientManagementService.Domain.ValueObject;
using PatientManagementService.Infrastructure.Data;


namespace PatientManagementService.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientDbContext _context;

        public PatientRepository(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Domain.Entities.Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients
                .Include(p => p.DossierMedical)
                    .ThenInclude(d => d.Documents)
                .ToListAsync();
        }


        public async Task<Domain.Entities.Patient?> GetPatientByIdAsync(Guid id)
        {
            return await _context.Patients
                .Include(p => p.DossierMedical)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddPatientAsync(Domain.Entities.Patient patient)
        {  
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePatientAsync(Domain.Entities.Patient patient)
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

        public async Task<IEnumerable<Domain.Entities.Patient>> GetPatientsByNameAsync(string? name, string? lastname)
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

        
        public async Task<int> GetStatistiquesAsync(DateTime dateDebut, DateTime dateFin)
        {
            var count = await _context.Patients
                .Where(p => p.DateCreation.Date >= dateDebut.Date && p.DateCreation.Date <= dateFin.Date)
                .CountAsync();

            return count;
        }
        
    }
}
