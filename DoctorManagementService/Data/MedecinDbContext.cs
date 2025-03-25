using DoctorManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorManagementService.Data
{
    public class MedecinDbContext : DbContext
    {
        public MedecinDbContext(DbContextOptions<MedecinDbContext> options) : base(options) { }

        public DbSet<Medecin> Medecins { get; set; }
    }
}
