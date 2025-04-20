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
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Paiement>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            base.OnModelCreating(modelBuilder);
        }
    }
}
