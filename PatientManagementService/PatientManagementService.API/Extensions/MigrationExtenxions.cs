using Microsoft.EntityFrameworkCore;
using PatientManagementService.Infrastructure.Data;

namespace PatientManagementService.API.Extensions
{
    public static class MigrationExtenxions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PatientDbContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
