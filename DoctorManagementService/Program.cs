using DoctorManagementService.Data;
using DoctorManagementService.Repositories;
using DoctorManagementService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MedecinDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MedecinDatabase")));

builder.Services.AddScoped<IMedecinRepository, MedecinRepository>();
builder.Services.AddScoped<IMedecinService, MedecinService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
