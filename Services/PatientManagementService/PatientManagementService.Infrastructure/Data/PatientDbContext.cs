using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientManagementService.Domain.Common;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Infrastructure.Data
{
    public class PatientDbContext : DbContext
    {
        private readonly IMediator _mediator;
        public PatientDbContext(DbContextOptions<PatientDbContext> options, IMediator mediator) : base(options) 
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<DossierMedical> DossiersMedicaux { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.DossierMedical)
                .WithOne(dm => dm.Patient)
                .HasForeignKey<DossierMedical>(dm => dm.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DossierMedical>()
                .HasMany(dm => dm.Documents)
                .WithOne(d => d.DossierMedical)
                .HasForeignKey(d => d.DossierMedicalId)
                .OnDelete(DeleteBehavior.Cascade);

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
