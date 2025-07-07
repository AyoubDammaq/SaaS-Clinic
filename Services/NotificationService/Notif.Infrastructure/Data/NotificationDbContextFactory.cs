using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Notif.Infrastructure.Data
{
    public class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
    {
        public NotificationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NotificationDbContext>();

            // 🔁 Met ici ta vraie chaîne de connexion
            optionsBuilder.UseSqlServer("Data Source=host.docker.internal,1435;Database=NotificationDb;User ID=sa;Password=azerty@123456;Encrypt=False;TrustServerCertificate=True;");

            return new NotificationDbContext(optionsBuilder.Options);
        }
    }
}
