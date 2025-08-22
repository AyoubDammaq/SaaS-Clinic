using Polly;
using Polly.Extensions.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using Reporting.Application.Interfaces;
using Reporting.Application.Mapping;
using Reporting.Application.Services;
using Reporting.Domain.Interfaces;
using Reporting.Infrastructure.Handlers;
using Reporting.Infrastructure.Repositories;
using Scalar.AspNetCore;
using System.Text;
using Notif.Domain.Interfaces;
using Notif.Application.Interfaces;
using Notif.Infrastructure.Repositories;
using Notif.Application.Services;
using Microsoft.EntityFrameworkCore;
using Notif.Infrastructure.Data;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;


// Add services to the container.
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8095);
    serverOptions.ListenAnyIP(8096);
});

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NotificationDatabase")));


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddHttpClient<IConsultationStateRepository, ConsultationStateRepository>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5011"); // Adresse de base du microservice Consultation
});

//notification
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationApplicationService, NotificationApplicationService>();

builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IReportingRepository, ReportingRepository>();
builder.Services.AddScoped<IConsultationStateRepository, ConsultationStateRepository>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddMemoryCache();

builder.Services.AddAutoMapper(typeof(ReportingMappingProfile).Assembly);


builder.Services.AddTransient<HttpLoggingHandler>();
builder.Services.AddTransient<BearerTokenHandler>();

builder.Services.AddHttpClient<IReportingRepository, ReportingRepository>()
    .AddHttpMessageHandler<HttpLoggingHandler>()
    .AddHttpMessageHandler<BearerTokenHandler>();
    //.AddPolicyHandler(GetRetryPolicy())
    //.AddPolicyHandler(GetCircuitBreakerPolicy());


var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException("La clé JWT (Jwt:Key) n'est pas configurée.");

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
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
}

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseMetricServer();
app.UseHttpMetrics();

app.MapControllers();

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2s, 4s, 8s
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds}s due to {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
            });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30),
            onBreak: (outcome, timespan) =>
            {
                Console.WriteLine($"Circuit opened for {timespan.TotalSeconds}s due to {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
            },
            onReset: () => Console.WriteLine("Circuit closed - ready to send requests again"),
            onHalfOpen: () => Console.WriteLine("Circuit half-open - testing health...")
        );
}

public partial class Program { }
