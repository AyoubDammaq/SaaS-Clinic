using Facturation.Domain.Entities;
using Facturation.Domain.Interfaces;
using Facturation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Facturation.Infrastructure.Repositories
{
    public class PaiementRepository : IPaiementRepository
    {
        private readonly FacturationDbContext _context;

        public PaiementRepository(FacturationDbContext context)
        {
            _context = context;
        }

        public async Task<Paiement?> GetByFactureIdAsync(Guid factureId)
        {
            return await _context.Paiements
                .Include(p => p.CardDetails)
                .FirstOrDefaultAsync(p => p.FactureId == factureId);
        }

        public async Task AddAsync(Paiement paiement)
        {
            _context.Paiements.Add(paiement);
            if (paiement.CardDetails != null)
            {
                _context.CardDetails.Add(paiement.CardDetails);
            }
            await _context.SaveChangesAsync();
        }
        public async Task<Paiement?> GetDernierPaiementByPatientIdAsync(Guid patientId)
        {
            return await _context.Paiements
                .Include(p => p.Facture)
                .Where(p => p.Facture != null && p.Facture.PatientId == patientId)
                .OrderByDescending(p => p.DatePaiement)
                .FirstOrDefaultAsync();
        }
    }
}
