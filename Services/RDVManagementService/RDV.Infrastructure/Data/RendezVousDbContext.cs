using MediatR;
using Microsoft.EntityFrameworkCore;
using RDV.Domain.Common;
using RDV.Domain.Entities;

namespace RDV.Infrastructure.Data
{
    public class RendezVousDbContext : DbContext
    {
        private readonly IMediator _mediator;
        public RendezVousDbContext(DbContextOptions<RendezVousDbContext> options, IMediator mediator) : base(options) 
        { 
            _mediator = mediator;           
        }
        public DbSet<RendezVous> RendezVous { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RendezVous>()
                .Property(r => r.DateCreation)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<RendezVous>()
                .Property(r => r.Statut)
                .HasConversion<string>();
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