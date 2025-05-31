using Facturation.Domain.Interfaces;
using Facturation.Infrastructure.Repositories;
using Facturation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Facturation.API.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Scalar.AspNetCore;
using Facturation.Application.FactureService.Commands.AddFacture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8093);
    serverOptions.ListenAnyIP(8094);
});


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<FacturationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FactureDatabase")));

builder.Services.AddScoped<IFactureRepository, FactureRepository>();
builder.Services.AddScoped<IPaiementRepository, PaiementRepository>();


builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(AddFactureCommand).Assembly));


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


app.UseAuthorization();

app.MapControllers();

app.Run();
