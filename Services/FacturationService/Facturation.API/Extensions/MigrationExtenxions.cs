using Facturation.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Facturation.API.Extensions
{
    public static class MigrationExtenxions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FacturationDbContext>();

            var retryCount = 0;
            const int maxRetry = 5;

            while (retryCount < maxRetry)
            {
                try
                {
                    db.Database.Migrate();
                    break;
                }
                catch (SqlException)
                {
                    retryCount++;
                    Thread.Sleep(5000); // attend 5 sec avant de réessayer
                }
            }
        }
    }
}
