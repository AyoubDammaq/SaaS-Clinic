using ConsultationManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultationManagementService.Data
{
    public class ConsultationDbContext : DbContext
    {
        public ConsultationDbContext(DbContextOptions<ConsultationDbContext> options) : base(options) { }

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
    }
}
