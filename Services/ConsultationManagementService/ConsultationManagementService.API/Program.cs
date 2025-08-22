using ConsultationManagementService.API.Extensions;
using ConsultationManagementService.Application.Commands.CreateConsultation;
using ConsultationManagementService.Application.Interfaces;
using ConsultationManagementService.Application.Mappings;
using ConsultationManagementService.Application.Services;
using ConsultationManagementService.Data;
using ConsultationManagementService.Domain.Interfaces.Messaging;
using ConsultationManagementService.Infrastructure.Messaging;
using ConsultationManagementService.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8091);
    serverOptions.ListenAnyIP(8092);
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Vérifier que la chaîne de connexion existe
if (string.IsNullOrEmpty(builder.Configuration.GetConnectionString("ConsultationDatabase")))
{
    throw new InvalidOperationException("La chaîne de connexion 'ConsultationDatabase' est manquante dans la configuration.");
}

// Configuration de la base de données
builder.Services.AddDbContext<ConsultationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConsultationDatabase"),
        sqlOptions =>
        {
            sqlOptions.CommandTimeout(180); // Timeout in seconds (3 minutes)
        }));


// Injection des dépendances
builder.Services.AddScoped<IConsultationRepository, ConsultationRepository>();
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

builder.Services.AddHttpClient<IDoctorHttpClient, DoctorHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://doctorservice:8085"); // nom DNS Docker
});

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(CreateConsultationCommand).Assembly));

// Configuration de l'authentification JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"C:\keys"))
    .SetApplicationName("MyApp");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.ApplyMigrations();
}

var wwwrootPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(wwwrootPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = ""
});

app.UseCors("AllowAllOrigins");

app.UseAuthentication(); // Ajout du middleware d'authentification

app.UseAuthorization();

app.UseMetricServer();
app.UseHttpMetrics();

app.MapControllers();

// Démarrage de l'application
app.Run();
