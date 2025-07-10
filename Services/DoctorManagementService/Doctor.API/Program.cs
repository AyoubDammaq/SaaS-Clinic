using Doctor.API.Extensions;
using Doctor.Application.AvailibilityServices.Commands.AjouterDisponibilite;
using Doctor.Application.AvailibilityServices.Commands.UpdateDisponibilite;
using Doctor.Application.Behaviors;
using Doctor.Application.Interfaces;
using Doctor.Application.Services;
using Doctor.Domain.Interfaces;
using Doctor.Domain.Interfaces.Messaging;
using Doctor.Infrastructure.Data;
using Doctor.Infrastructure.Messaging;
using Doctor.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8085);
    serverOptions.ListenAnyIP(8086);
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<MedecinDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MedecinDatabase")));

builder.Services.AddScoped<IMedecinRepository, MedecinRepository>();
builder.Services.AddScoped<IDisponibiliteRepository, DisponibiliteRepository>();
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

builder.Services.AddHttpClient<IRendezVousHttpClient, RendezVousHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://rdvservice:8089"); // nom DNS Docker
});


builder.Services.AddHttpClient<IMedecinRepository, MedecinRepository>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AjouterDisponibiliteCommand).Assembly));
// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<AjouterDisponibiliteCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateDisponibiliteCommandValidator>();

// Ajout du comportement de validation
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.ApplyMigrations();
}

app.UseCors("AllowAllOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
