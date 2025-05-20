using Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Doctor.API.Extensions
{
    public static class MigrationExtenxions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MedecinDbContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
