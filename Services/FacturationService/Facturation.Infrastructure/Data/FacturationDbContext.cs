using Facturation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Facturation.Infrastructure.Data
{
    public class FacturationDbContext : DbContext
    {
        public FacturationDbContext(DbContextOptions<FacturationDbContext> options) : base(options) { }

        public DbSet<Facture> Factures { get; set; }

        public DbSet<Paiement> Paiements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Facture>()
                .Property(f => f.MontantTotal)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Paiement>()
                .HasOne(p => p.Facture)
                .WithOne(f => f.Paiement)
                .HasForeignKey<Paiement>(p => p.FactureId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
