using ConsultationManagementService.Data;
using ConsultationManagementService.Repositories;
using ConsultationManagementService.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Vérifier que la chaîne de connexion existe
if (string.IsNullOrEmpty(builder.Configuration.GetConnectionString("ConsultationDatabase")))
{
    throw new InvalidOperationException("La chaîne de connexion 'ConsultationDatabase' est manquante dans la configuration.");
}

// Configuration de la base de données
builder.Services.AddDbContext<ConsultationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConsultationDatabase")));

// Injection des dépendances
builder.Services.AddScoped<IConsultationRepository, ConsultationRepository>();
builder.Services.AddScoped<IConsultationService, ConsultationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware de sécurité et routage
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Démarrage de l'application
app.Run();
