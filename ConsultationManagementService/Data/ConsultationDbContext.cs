using ConsultationManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultationManagementService.Data
{
    public class ConsultationDbContext : DbContext
    {
        public ConsultationDbContext(DbContextOptions<ConsultationDbContext> options) : base(options) { }

        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<DocumentMedical> DocumentsMedicaux { get; set; }
    }
}
