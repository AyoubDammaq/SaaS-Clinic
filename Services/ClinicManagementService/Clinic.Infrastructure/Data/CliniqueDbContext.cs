using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Data
{
    public class CliniqueDbContext : DbContext
    {
        private readonly Guid _tenantId;
        public CliniqueDbContext(DbContextOptions<CliniqueDbContext> options)
            : base(options){}

        public DbSet<Clinique> Cliniques { get; set; }
    }
}
