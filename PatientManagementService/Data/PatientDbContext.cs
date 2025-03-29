using Microsoft.EntityFrameworkCore;
using PatientManagementService.Models;
using System.Collections.Generic;

namespace PatientManagementService.Data
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
    }
}
