using Microsoft.EntityFrameworkCore;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;
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


        public async Task<int> GetStatistiquesAsync(DateTime dateDebut, DateTime dateFin)
        {
            var count = await _context.Patients
                .Where(p => p.DateCreation.Date >= dateDebut.Date && p.DateCreation.Date <= dateFin.Date)
                .CountAsync();

            return count;
        }

        public async Task<int> CountTotalPatientsAsync()
        {
            return await _context.Patients.CountAsync();
        }

        public async Task<Dictionary<string, int>> GetNombreDeNouveauxPatientsParMoisAsync(DateTime date)
        {
            var startDate = new DateTime(date.Year, 1, 1);
            var endDate = new DateTime(date.Year, 12, 31);
            var result = await _context.Patients
                .Where(p => p.DateCreation >= startDate && p.DateCreation <= endDate)
                .GroupBy(p => new { p.DateCreation.Year, p.DateCreation.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();
            return result.ToDictionary(
                x => new DateTime(date.Year, x.Month, 1).ToString("MMMM"),
                x => x.Count
            );
        }

        public async Task<bool> LinkUserToPatientEntityAsync(Guid patientId, Guid userId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null)
            {
                return false; // Patient not found
            }
            patient.UserId = userId; // Assuming UserId is a property in Patient entity
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return true;

        }
    }   
}
