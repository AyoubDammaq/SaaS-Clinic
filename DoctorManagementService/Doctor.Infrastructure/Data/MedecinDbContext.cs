using Doctor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Infrastructure.Data
{
    public class MedecinDbContext : DbContext
    {
        public MedecinDbContext(DbContextOptions<MedecinDbContext> options) : base(options) { }

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

    }
}
