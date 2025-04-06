using ClinicManagementService.Data;
using ClinicManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementService.Repositories
{
    public class CliniqueRepository : ICliniqueRepository
    {
        private readonly CliniqueDbContext _context;

        public CliniqueRepository(CliniqueDbContext context)
        {
            _context = context;
        }

        public Task<List<Clinique>> GetAllAsync() 
        {
            return _context.Cliniques.ToListAsync();
        }

        public Task<Clinique?> GetByIdAsync(Guid id) 
        {
            return _context.Cliniques.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Clinique clinique) 
        {
            await _context.Cliniques.AddAsync(clinique);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Clinique clinique) 
        {
            _context.Cliniques.Update(clinique);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id) 
        {
            var clinique = await GetByIdAsync(id);
            if (clinique != null)
            {
                _context.Cliniques.Remove(clinique);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Clinique?> GetByNameAsync(string name) 
        { 
            return await _context.Cliniques.FirstOrDefaultAsync(c => c.Nom == name); 
        }

        public async Task<Clinique?> GetByAddressAsync(string address)
        {
            return await _context.Cliniques.FirstOrDefaultAsync(c => c.Adresse == address);
        }
    }
}
