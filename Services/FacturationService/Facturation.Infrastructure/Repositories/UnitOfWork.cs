using Facturation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Facturation.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FacturationDbContext _context;
        private readonly IFactureRepository _factureRepository;
        private readonly IPaiementRepository _paiementRepository;
        private IDbContextTransaction _currentTransaction;

        public UnitOfWork(FacturationDbContext context, IFactureRepository factureRepository, IPaiementRepository paiementRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _factureRepository = factureRepository ?? throw new ArgumentNullException(nameof(factureRepository));
            _paiementRepository = paiementRepository ?? throw new ArgumentNullException(nameof(paiementRepository));
        }

        public IFactureRepository FactureRepository => _factureRepository;
        public IPaiementRepository PaiementRepository => _paiementRepository;

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                throw new InvalidOperationException("Une transaction est déjà en cours.");

            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                _currentTransaction?.Commit();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
        }
    }
}