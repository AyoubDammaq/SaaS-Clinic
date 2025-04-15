using ConsultationManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultationManagementService.Data
{
    public class ConsultationDbContext : DbContext
    {
        public ConsultationDbContext(DbContextOptions<ConsultationDbContext> options) : base(options) { }

        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<DocumentMedical> DocumentsMedicaux { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Consultation>()
                .HasMany(c => c.Documents)
                .WithOne(d => d.Consultation)
                .HasForeignKey(d => d.ConsultationId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
