using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Repositories
{
    public class DisponibiliteRepository : IDisponibiliteRepository
    {
        private readonly MedecinDbContext _context;

        public DisponibiliteRepository(MedecinDbContext context)
        {
            _context = context;
        }

        public async Task AjouterDisponibiliteAsync(Disponibilite nouvelleDispo)
        {
            await _context.Disponibilites.AddAsync(nouvelleDispo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDisponibiliteAsync(Guid disponibiliteId, Disponibilite disponibilite)
        {
            var existingDisponibilite = await ObtenirDisponibiliteParIdAsync(disponibiliteId);
            if (existingDisponibilite != null)
            {
                existingDisponibilite.Jour = disponibilite.Jour;
                existingDisponibilite.HeureDebut = disponibilite.HeureDebut;
                existingDisponibilite.HeureFin = disponibilite.HeureFin;
                _context.Disponibilites.Update(existingDisponibilite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SupprimerDisponibiliteAsync(Guid disponibiliteId)
        {
            var disponibilite = await ObtenirDisponibiliteParIdAsync(disponibiliteId);
            if (disponibilite != null)
            {
                _context.Disponibilites.Remove(disponibilite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Disponibilite> ObtenirDisponibiliteParIdAsync(Guid disponibiliteId)
        {
            return await _context.Disponibilites
                .FirstOrDefaultAsync(d => d.Id == disponibiliteId);
        }

        public async Task<List<Disponibilite>> ObtenirToutesDisponibilitesAsync()
        {
            return await _context.Disponibilites.ToListAsync();
        }

        public async Task<List<Disponibilite>> ObtenirDisponibilitesParMedecinIdAsync(Guid medecinId)
        {
            return await _context.Disponibilites
                .Where(d => d.MedecinId == medecinId)
                .ToListAsync();
        }

        public async Task<List<Disponibilite>> ObtenirDisponibilitesParJourAsync(Guid medecinId, DayOfWeek jour)
        {
            return await _context.Disponibilites
                .Where(d => d.MedecinId == medecinId && d.Jour == jour)
                .ToListAsync();
        }


        public async Task<List<Medecin>> ObtenirMedecinsDisponiblesAsync(DateTime date, TimeSpan? heureDebut, TimeSpan? heureFin)
        {
            var day = date.DayOfWeek;

            return await _context.Medecins
                .Include(m => m.Disponibilites)
                .Where(m => m.Disponibilites.Any(d =>
                    d.Jour == day &&
                    (!heureDebut.HasValue || d.HeureDebut <= heureDebut.Value) &&
                    (!heureFin.HasValue || d.HeureFin >= heureFin.Value)))
                .ToListAsync();
        }

        public async Task<bool> EstDisponibleAsync(Guid medecinId, DateTime dateTime)
        {
            var day = dateTime.DayOfWeek;
            var time = dateTime.TimeOfDay;

            return await _context.Disponibilites.AnyAsync(d =>
                d.MedecinId == medecinId &&
                d.Jour == day &&
                d.HeureDebut <= time &&
                d.HeureFin > time);
        }

        public async Task<bool> VerifieChevauchementAsync(Disponibilite dispo)
        {
            return await _context.Disponibilites.AnyAsync(d =>
                d.MedecinId == dispo.MedecinId &&
                d.Id != dispo.Id &&  // exclude self for update
                d.Jour == dispo.Jour &&
                ((dispo.HeureDebut < d.HeureFin) && (d.HeureDebut < dispo.HeureFin))
            );
        }

        public async Task<TimeSpan> ObtenirTempsTotalDisponibleAsync(Guid medecinId, DateTime dateDebut, DateTime dateFin)
        {
            // Calculate total available time for the doctor between dateDebut and dateFin (inclusive)
            var total = TimeSpan.Zero;

            // Get days in the range (weekdays)
            var dates = Enumerable.Range(0, (dateFin - dateDebut).Days + 1)
                .Select(offset => dateDebut.AddDays(offset).DayOfWeek)
                .Distinct()
                .ToList();

            var disponibilites = await _context.Disponibilites
                .Where(d => d.MedecinId == medecinId && dates.Contains(d.Jour))
                .ToListAsync();

            foreach (var dispo in disponibilites)
            {
                total += dispo.HeureFin - dispo.HeureDebut;
            }

            return total;
        }

        public async Task SupprimerDisponibilitesParMedecinIdAsync(Guid medecinId)
        {
            var disponibilites = await ObtenirDisponibilitesParMedecinIdAsync(medecinId);
            if (disponibilites != null && disponibilites.Count > 0)
            {
                _context.Disponibilites.RemoveRange(disponibilites);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Disponibilite>> ObtenirDisponibilitesDansIntervalleAsync(Guid medecinId, DateTime start, DateTime end)
        {
            return await _context.Disponibilites
                .Where(d => d.MedecinId == medecinId && d.HeureDebut >= start.TimeOfDay && d.HeureFin <= end.TimeOfDay)
                .ToListAsync();
        }
    }
}
