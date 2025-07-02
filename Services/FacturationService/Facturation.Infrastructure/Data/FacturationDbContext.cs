using Facturation.Domain.Common;
using Facturation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Facturation.Infrastructure.Data
{
    public class FacturationDbContext : DbContext
    {
        private readonly IMediator _mediator;
        public FacturationDbContext(DbContextOptions<FacturationDbContext> options, IMediator mediator) : base(options) 
        {
            _mediator = mediator;
        }

        public DbSet<Facture> Factures { get; set; }

        public DbSet<Paiement> Paiements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Facture>()
                .Property(f => f.MontantTotal)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Facture>()
                .Property(f => f.MontantPaye)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Paiement>()
                .HasOne(p => p.Facture)
                .WithOne(f => f.Paiement)
                .HasForeignKey<Paiement>(p => p.FactureId);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1. Publier tous les Domain Events AVANT de sauvegarder
            var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var allEvents = new List<INotification>();

            foreach (var entity in entitiesWithEvents)
            {
                allEvents.AddRange(entity.DomainEvents);
                entity.ClearDomainEvents();
            }

            foreach (var domainEvent in allEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            // 2. Ensuite enregistrer les modifications
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}
