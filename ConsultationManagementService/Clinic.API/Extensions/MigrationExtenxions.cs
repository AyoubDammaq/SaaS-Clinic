using ConsultationManagementService.Data;
using Microsoft.EntityFrameworkCore;

namespace Consultation.API.Extensions
{
    public static class MigrationExtenxions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ConsultationDbContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
