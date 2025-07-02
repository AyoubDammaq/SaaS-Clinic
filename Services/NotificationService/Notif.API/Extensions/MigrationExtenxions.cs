using Microsoft.EntityFrameworkCore;
using Notif.Infrastructure.Data;

namespace Notif.API.Extensions
{
    public static class MigrationExtenxions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
