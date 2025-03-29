using Microsoft.EntityFrameworkCore;
using RDVManagementService.Models;
using System.Collections.Generic;

namespace RDVManagementService.Data
{
    public class RendezVousDbContext : DbContext
    {
        public RendezVousDbContext(DbContextOptions<RendezVousDbContext> options) : base(options) { }

        public DbSet<RendezVous> RendezVous { get; set; }
    }
}
