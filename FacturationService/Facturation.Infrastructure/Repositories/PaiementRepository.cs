

using Facturation.Domain.Entities;
using Facturation.Domain.Interfaces;
using Facturation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

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
                .FirstOrDefaultAsync(p => p.FactureId == factureId);
        }

        public async Task AddAsync(Paiement paiement)
        {
            _context.Paiements.Add(paiement);
            await _context.SaveChangesAsync();
        }


    }

}
