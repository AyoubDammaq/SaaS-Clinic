namespace Facturation.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        IFactureRepository FactureRepository { get; }
        IPaiementRepository PaiementRepository { get; }
    }
}