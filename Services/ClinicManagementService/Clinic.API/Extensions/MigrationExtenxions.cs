using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.Extensions
{
    public static class MigrationExtenxions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CliniqueDbContext>();

            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }
        }
    }
}
