using Microsoft.EntityFrameworkCore;
using RDV.Domain.Entities;

namespace RDV.Infrastructure.Data
{
    public class RendezVousDbContext : DbContext
    {
        public RendezVousDbContext(DbContextOptions<RendezVousDbContext> options) : base(options) { }
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
    }
}