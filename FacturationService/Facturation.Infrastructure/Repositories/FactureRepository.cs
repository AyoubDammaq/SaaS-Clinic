using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using Facturation.Domain.ValueObjects;
using Facturation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Facturation.Infrastructure.Repositories
{
    public class FactureRepository : IFactureRepository
    {
        public readonly FacturationDbContext _context;

        public FactureRepository(FacturationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Facture?> GetFactureByIdAsync(Guid id)
        {
            return await _context.Factures.FindAsync(id);
        }

        public async Task<IEnumerable<Facture>> GetAllFacturesAsync()
        {
            return await _context.Factures.ToListAsync();
        }

        public async Task AddFactureAsync(Facture facture)
        {
            await _context.Factures.AddAsync(facture);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFactureAsync(Facture facture)
        {
            _context.Factures.Update(facture);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFactureAsync(Guid id)
        {
            var facture = await GetFactureByIdAsync(id);
            if (facture != null)
            {
                _context.Factures.Remove(facture);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Facture>> GetAllFacturesByRangeOfDateAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Factures
                .Where(f => f.DateEmission >= startDate && f.DateEmission <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Facture>> GetAllFacturesByStateAsync(FactureStatus status)
        {
            return await _context.Factures
                .Where(f => f.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Facture>> GetAllFacturesByPatientIdAsync(Guid patientId)
        {
            return await _context.Factures
                .Where(f => f.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Facture>> GetAllFacturesByClinicIdAsync(Guid clinicId)
        {
            return await _context.Factures
                .Where(f => f.ClinicId == clinicId)
                .ToListAsync();
        }

        public async Task<IEnumerable<FactureStats>> GetNombreDeFactureByStatusAsync()
        {
            return await _context.Factures
               .GroupBy(f => f.Status)
               .Select(g => new FactureStats
               {
                   Cle = g.Key.ToString(),
                   Nombre = g.Count()
               })
               .ToListAsync();
        }

        public async Task<IEnumerable<FactureStats>> GetNombreDeFactureParCliniqueAsync()
        {
            return await _context.Factures
                .GroupBy(f => f.ClinicId)
                .Select(g => new FactureStats
                {
                    Cle = g.Key.ToString(), 
                    Nombre = g.Count()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FactureStats>> GetNombreDeFacturesByStatusParCliniqueAsync()
        {
            return await _context.Factures
                .GroupBy(f => new { f.ClinicId, f.Status })
                .Select(g => new FactureStats
                {
                    Cle = g.Key.ToString(), 
                    Nombre = g.Count()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FactureStats>> GetNombreDeFacturesByStatusDansUneCliniqueAsync(Guid cliniqueId)
        {
            return await _context.Factures
                .Where(f => f.ClinicId == cliniqueId)
                .GroupBy(f => f.ClinicId)
                .Select(g => new FactureStats
                {
                    Cle = g.Key.ToString(),
                    Nombre = g.Count()
                })
                .ToListAsync();
        }
    }
}
