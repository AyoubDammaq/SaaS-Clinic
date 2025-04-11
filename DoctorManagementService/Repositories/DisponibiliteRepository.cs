using DoctorManagementService.Data;
using DoctorManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorManagementService.Repositories
{
    public class DisponibiliteRepository : IDisponibiliteRepository 
    {
        private readonly MedecinDbContext _context;

        public DisponibiliteRepository(MedecinDbContext context)
        {
            _context = context;
        }

        public async Task AjouterDisponibiliteAsync(Guid medecinId, Disponibilite nouvelleDispo)
        {
            var medecin = await _context.Medecins
                .Include(m => m.Disponibilites)
                .FirstOrDefaultAsync(m => m.Id == medecinId);

            if (medecin != null)
            {
                nouvelleDispo.MedecinId = medecinId;
                _context.Disponibilites.Add(nouvelleDispo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Disponibilite>> ObtenirDisponibilitesParMedecinId(Guid medecinId)
        {
            return await _context.Disponibilites
                .Where(d => d.MedecinId == medecinId)
                .ToListAsync();
        }

        public async Task<Disponibilite> ObtenirDisponibiliteParId(Guid disponibiliteId)
        {
            return await _context.Disponibilites
                .FirstOrDefaultAsync(d => d.Id == disponibiliteId);
        }

        public async Task SupprimerDisponibiliteAsync(Guid disponibiliteId)
        {
            var disponibilite = await ObtenirDisponibiliteParId(disponibiliteId);
            if (disponibilite != null)
            {
                _context.Disponibilites.Remove(disponibilite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Disponibilite>> ObtenirDisponibilites()
        {
            return await _context.Disponibilites.ToListAsync();
        }

        public async Task<List<Medecin>> ObtenirMedecinsDisponiblesAsync(DateTime date, TimeSpan? heureDebut, TimeSpan? heureFin)
        {
            return await _context.Medecins
                .Include(m => m.Disponibilites)
                .Where(m => m.Disponibilites.Any(d =>
                    d.Jour == date.DayOfWeek &&
                    (!heureDebut.HasValue || d.HeureDebut < heureDebut.Value) &&
                    (!heureFin.HasValue || d.HeureFin > heureFin.Value)))
                .ToListAsync();
        }
    }
}
