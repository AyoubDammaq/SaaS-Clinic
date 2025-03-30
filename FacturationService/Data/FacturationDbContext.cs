using FacturationService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FacturationService.Data
{
    public class FacturationDbContext : DbContext
    {
        public FacturationDbContext(DbContextOptions<FacturationDbContext> options) : base(options) { }

        public DbSet<Facture> Factures { get; set; }
    }
}
