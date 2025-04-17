using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Clinic.Infrastructure.Repositories
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

        public async Task<List<Clinique?>> GetByNameAsync(string name)
        {
            return await _context.Cliniques.Where(c => c.Nom.ToLower().Contains(name)).ToListAsync();
        }

        public async Task<List<Clinique?>> GetByAddressAsync(string address)
        {
            return await _context.Cliniques.Where(c => c.Adresse.ToLower().Contains(address)).ToListAsync();
        }
    }
}
