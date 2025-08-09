using Facturation.Domain.Entities;
using Facturation.Domain.Interfaces;
using Facturation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Facturation.Infrastructure.Repositories
{
    public class TarificationConsultationRepository : ITarificationConsultationRepository
    {
        private readonly FacturationDbContext _context;

        public TarificationConsultationRepository(FacturationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal?> GetMontantAsync(Guid clinicId, int typeConsultation)
        {
            return await _context.TarifsConsultation
                .Where(t => t.ClinicId == clinicId && t.ConsultationType == (TypeConsultation)typeConsultation)
                .Select(t => (decimal?)t.Prix)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(TarifConsultation tarif)
        {
            _context.TarifsConsultation.Add(tarif);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TarifConsultation tarif)
        {
            _context.TarifsConsultation.Update(tarif);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var tarif = await _context.TarifsConsultation.FindAsync(id);
            if (tarif != null)
            {
                _context.TarifsConsultation.Remove(tarif);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TarifConsultation>> GetAllAsync()
        {
            return await _context.TarifsConsultation.ToListAsync();
        }

        public async Task<TarifConsultation?> GetByIdAsync(Guid id)
        {
            return await _context.TarifsConsultation.FindAsync(id);
        }

        public async Task<IEnumerable<TarifConsultation>> GetByClinicIdAsync(Guid cliniqueId)
        {
            return await _context.TarifsConsultation
                .Where(t => t.ClinicId == cliniqueId)
                .ToListAsync();
        }
    }
}
