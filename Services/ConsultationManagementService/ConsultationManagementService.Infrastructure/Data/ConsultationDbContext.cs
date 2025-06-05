using ConsultationManagementService.Domain.Common;
using ConsultationManagementService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConsultationManagementService.Data
{
    public class ConsultationDbContext : DbContext
    {

        private readonly IMediator _mediator;
        public ConsultationDbContext(DbContextOptions<ConsultationDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Consultation> Consultations { get; set; } = null!;
        public DbSet<DocumentMedical> DocumentsMedicaux { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<Consultation>()
                .HasMany(c => c.Documents)
                .WithOne(d => d.Consultation)
                .HasForeignKey(d => d.ConsultationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ajout des configurations de clés primaires
            modelBuilder.Entity<Consultation>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<DocumentMedical>()
                .HasKey(d => d.Id);

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
