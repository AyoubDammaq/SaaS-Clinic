using Doctor.Domain.Common;
using Doctor.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Data
{
    public class MedecinDbContext : DbContext
    {
        private readonly IMediator _mediator;
        public MedecinDbContext(DbContextOptions<MedecinDbContext> options, IMediator mediator) : base(options) 
        {
            _mediator = mediator;
        }

        public DbSet<Medecin> Medecins { get; set; }
        public DbSet<Disponibilite> Disponibilites { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation entre Medecin et Disponibilite
            modelBuilder.Entity<Medecin>()
                .HasMany(m => m.Disponibilites)
                .WithOne(d => d.Medecin)
                .HasForeignKey(d => d.MedecinId)
                .OnDelete(DeleteBehavior.Cascade);
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
