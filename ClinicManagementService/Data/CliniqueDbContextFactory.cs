using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementService.Data
{
    /*
    public class CliniqueDbContextFactory : IDesignTimeDbContextFactory<CliniqueDbContext>
    {

        public CliniqueDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CliniqueDbContext>();
            optionsBuilder.UseSqlServer("Data Source=BEST-TECHNOLOGY\\MSSQLSERVER01;Database=ClinicDb;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");

            // Lors des migrations, on utilise Guid.Empty comme valeur par défaut
            return new CliniqueDbContext(optionsBuilder.Options, Guid.Empty);
        }
    }
    */
}
