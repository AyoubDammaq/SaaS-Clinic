using Microsoft.EntityFrameworkCore;
using Notification.Application.Services;
using Notification.Infrastructure.Data;
using Notification.Infrastructure.Repositories;
using Notification.Application.Interfaces;
using Notification.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//  1. Configuration de la DB (ex: SQL Server / PostgreSQL / SQLite)
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NotificationDatabase"))
// ou .UseNpgsql(...) / .UseSqlite(...) selon la DB
);

//  2. Injection des dépendances
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

//  3. Controllers & JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // PascalCase si tu préfères
    });

//  4. Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  5. CORS (optionnel)
/*
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
*/

var app = builder.Build();

//  Middleware
//app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseAuthentication(); // Si tu as l’auth plus tard
// app.UseAuthorization();

app.MapControllers();

app.Run();
