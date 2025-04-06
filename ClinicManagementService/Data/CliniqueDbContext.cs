using ClinicManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementService.Data
{
    public class CliniqueDbContext : DbContext
    {
        private readonly Guid _tenantId;
        public CliniqueDbContext(DbContextOptions<CliniqueDbContext> options /*, Guid? tenantId = null*/)
            : base(options)
        {
            //_tenantId = tenantId ?? Guid.NewGuid();
        }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=BEST-TECHNOLOGY\\MSSQLSERVER01;Database=ClinicDb;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");
            }
        }
        */

        public DbSet<Clinique> Cliniques { get; set; }

        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Filtre global pour le multilocataire
            
            modelBuilder.Entity<Clinique>()
                .HasQueryFilter(c => c.TenantId == _tenantId);
            
        }
        */
    }
}

