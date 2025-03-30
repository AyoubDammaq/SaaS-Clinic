using Microsoft.EntityFrameworkCore;
using RessourceManagementService.Models;
using System.Collections.Generic;

namespace RessourceManagementService.Data
{
    public class RessourceMedicaleDbContext : DbContext
    {
        public RessourceMedicaleDbContext(DbContextOptions<RessourceMedicaleDbContext> options) : base(options) { }

        public DbSet<RessourceMedicale> RessourcesMedicales { get; set; }
    }
}
