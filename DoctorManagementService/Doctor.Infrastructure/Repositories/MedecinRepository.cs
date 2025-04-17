
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Repositories
{
    public class MedecinRepository : IMedecinRepository
    {
        private readonly MedecinDbContext _context;
        public MedecinRepository(MedecinDbContext context)
        {
            _context = context;
        }
        public async Task<Medecin> GetByIdAsync(Guid id)
        {
            return await _context.Medecins
                .Include(m => m.Disponibilites)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<List<Medecin>> GetAllAsync()
        {
            return await _context.Medecins
                .Include(m => m.Disponibilites)
                .ToListAsync();
        }
        public async Task AddAsync(Medecin medecin)
        {
            await _context.Medecins.AddAsync(medecin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Medecin medecin)
        {
            _context.Medecins.Update(medecin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var medecin = await GetByIdAsync(id);
            if (medecin != null)
            {
                _context.Medecins.Remove(medecin);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Medecin>> FilterBySpecialiteAsync(string specialite)
        {
            return await _context.Medecins
                .Include(m => m.Disponibilites)
                .Where(m => (string.IsNullOrEmpty(specialite) || m.Specialite == specialite))
                .ToListAsync();
        }

        public async Task<List<Medecin>> FilterByNameOrPrenomAsync(string name, string prenom)
        {
            name = name?.Trim().ToLower();
            prenom = prenom?.Trim().ToLower();

            return await _context.Medecins
                .Where(m =>
                    (string.IsNullOrEmpty(name) || m.Nom.ToLower().Contains(name)) &&
                    (string.IsNullOrEmpty(prenom) || m.Prenom.ToLower().Contains(prenom))
                )
                .Include(m => m.Disponibilites)
                .ToListAsync();
        }

        public async Task<List<Medecin>> GetMedecinByCliniqueIdAsync(Guid cliniqueId)
        {
            return await _context.Medecins
                .Where(m => m.CliniqueId == cliniqueId)
                .Include(m => m.Disponibilites)
                .ToListAsync();
        }
        public async Task AttribuerMedecinAUneCliniqueAsync(Guid medecinId, Guid cliniqueId)
        {
            var medecin = await GetByIdAsync(medecinId);
            if (medecin != null)
            {
                medecin.CliniqueId = cliniqueId;
                await UpdateAsync(medecin);
            }
        }
        public async Task DesabonnerMedecinDeCliniqueAsync(Guid medecinId)
        {
            var medecin = await GetByIdAsync(medecinId);
            if (medecin != null)
            {
                medecin.CliniqueId = null;
                await UpdateAsync(medecin);
            }
        }
    }
}
