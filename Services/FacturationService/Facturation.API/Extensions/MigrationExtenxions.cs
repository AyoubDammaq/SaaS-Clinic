using Facturation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Facturation.API.Extensions
{
    public static class MigrationExtenxions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FacturationDbContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
