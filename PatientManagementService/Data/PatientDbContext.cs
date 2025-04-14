using Microsoft.EntityFrameworkCore;
using PatientManagementService.Models;
using System.Collections.Generic;

namespace PatientManagementService.Data
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<DossierMedical> DossiersMedicaux { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<DossierMedical>()
                .HasKey(dm => dm.Id);
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.DossierMedical)
                .WithOne(dm => dm.Patient)
                .HasForeignKey<DossierMedical>(dm => dm.PatientId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
