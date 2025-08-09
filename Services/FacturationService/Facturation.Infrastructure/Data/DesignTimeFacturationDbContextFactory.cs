using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Facturation.Infrastructure.Data
{
    public class DesignTimeFacturationDbContextFactory : IDesignTimeDbContextFactory<FacturationDbContext>
    {
        public FacturationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FacturationDbContext>();

            // ⚠️ Remplace par ta vraie chaîne de connexion locale
            var connectionString = "Data Source=host.docker.internal,1435;Database=FactureDb;User ID=sa;Password=azerty@123456;Encrypt=False;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(connectionString);

            return new FacturationDbContext(optionsBuilder.Options, mediator: null);
        }
    }
}
