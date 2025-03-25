using ClinicManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementService.Data
{
    public class CliniqueDbContext : DbContext
    {
        public CliniqueDbContext(DbContextOptions<CliniqueDbContext> options) : base(options) { }

        public DbSet<Clinique> Cliniques { get; set; }
    }
}
