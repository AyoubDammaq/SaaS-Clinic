using Clinic.Domain.Common;
using Clinic.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Data
{
    public class CliniqueDbContext : DbContext
    {
        private readonly IMediator _mediator;
        public CliniqueDbContext(DbContextOptions<CliniqueDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Clinique> Cliniques { get; set; }

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
