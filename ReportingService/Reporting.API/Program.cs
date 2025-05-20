using Microsoft.AspNetCore.DataProtection;
using Reporting.Application.Interfaces;
using Reporting.Application.Services;
using Reporting.Domain.Interfaces;
using Reporting.Infrastructure.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8095);
    serverOptions.ListenAnyIP(8096);
});


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddHttpClient<IConsultationStateRepository, ConsultationStateRepository>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5011"); // Adresse de base du microservice Consultation
});

builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IReportingRepository, ReportingRepository>();
builder.Services.AddScoped<IConsultationStateRepository, ConsultationStateRepository>();
builder.Services.AddHttpContextAccessor();



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
}

app.UseCors("AllowAllOrigins");


app.UseAuthorization();

app.MapControllers();

app.Run();
