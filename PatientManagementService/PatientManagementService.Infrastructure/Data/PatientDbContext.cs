using Microsoft.EntityFrameworkCore;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Infrastructure.Data
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options) { }

        public DbSet<Domain.Entities.Patient> Patients { get; set; }
        public DbSet<DossierMedical> DossiersMedicaux { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Entities.Patient>()
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
    }
}
