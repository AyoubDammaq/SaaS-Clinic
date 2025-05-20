using Microsoft.EntityFrameworkCore;
using RDV.Infrastructure.Data;

namespace RDV.API.Extensions
{
    public static class MigrationExtenxions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RendezVousDbContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
