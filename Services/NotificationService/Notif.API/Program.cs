using Microsoft.EntityFrameworkCore;
using Notif.API.Extensions;
using Notif.Application.Interfaces;
using Notif.Application.Services;
using Notif.Domain.Interfaces;
using Notif.Infrastructure.Data;
using Notif.Infrastructure.Dispatching;
using Notif.Infrastructure.Repositories;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Notif.Infrastructure.Messaging.Consumers;
using Confluent.Kafka;
using Notif.Infrastructure.Channels;
using Notif.Infrastructure.Factories;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------
// 1. Ports d’écoute Kestrel
// ---------------------------------------------------
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8097); // API
    serverOptions.ListenAnyIP(8098); // Dashboard/worker/etc.
});

// ---------------------------------------------------
// 2. Configuration de la base de données
// ---------------------------------------------------
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("NotificationDatabase"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
);


// ---------------------------------------------------
// 3. Injection des dépendances (DI)
// ---------------------------------------------------
builder.Services.AddHttpClient();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
builder.Services.AddScoped<INotificationApplicationService, NotificationApplicationService>();
builder.Services.AddScoped<INotificationChannelFactory, NotificationChannelFactory>();
builder.Services.AddScoped<IUserPreferenceService, UserPreferenceService>();
builder.Services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
builder.Services.AddScoped<IUserEmailResolverService, UserEmailResolverService>();

builder.Services.AddTransient<SmsChannel>();
builder.Services.AddTransient<EmailChannel>();
builder.Services.AddTransient<PushChannel>();
builder.Services.AddTransient<InAppChannel>();

// Correction 1 : typage explicite du ConsumerConfig factory
builder.Services.AddSingleton<Func<string, ConsumerConfig>>(sp => consumerName =>
{
    var kafkaSection = builder.Configuration.GetSection("Kafka");
    var bootstrapServers = kafkaSection["BootstrapServers"];
    var autoOffsetResetValue = kafkaSection["AutoOffsetReset"];
    if (string.IsNullOrEmpty(autoOffsetResetValue))
        throw new Exception("Kafka:AutoOffsetReset n'est pas configuré.");

    var autoOffsetReset = Enum.Parse<AutoOffsetReset>(autoOffsetResetValue, true);
    var consumersSection = kafkaSection.GetSection("Consumers");

    var groupId = consumersSection.GetSection(consumerName)["GroupId"];
    if (string.IsNullOrEmpty(groupId))
    {
        throw new Exception($"GroupId not configured for consumer {consumerName}");
    }

    return new ConsumerConfig
    {
        BootstrapServers = bootstrapServers,
        GroupId = groupId,
        AutoOffsetReset = autoOffsetReset,
        EnableAutoCommit = false
    };
});

// ---------------------------------------------------
// Kafka ConsumerConfig factory
// ---------------------------------------------------
builder.Services.AddSingleton<Func<string, ConsumerConfig>>(sp => consumerName =>
{
    var kafkaSection = builder.Configuration.GetSection("Kafka");
    var bootstrapServers = kafkaSection["BootstrapServers"];
    var autoOffsetResetValue = kafkaSection["AutoOffsetReset"];
    if (string.IsNullOrEmpty(autoOffsetResetValue))
        throw new Exception("Kafka:AutoOffsetReset n'est pas configuré.");

    var autoOffsetReset = Enum.Parse<AutoOffsetReset>(autoOffsetResetValue, true);
    var consumersSection = kafkaSection.GetSection("Consumers");

    var groupId = consumersSection.GetSection(consumerName)["GroupId"];
    if (string.IsNullOrEmpty(groupId))
    {
        throw new Exception($"GroupId not configured for consumer {consumerName}");
    }

    return new ConsumerConfig
    {
        BootstrapServers = bootstrapServers,
        GroupId = groupId,
        AutoOffsetReset = autoOffsetReset,
        EnableAutoCommit = false
    };
});

// ---------------------------------------------------
// Injection explicite des consumers avec config spécifique
// ---------------------------------------------------
builder.Services.AddSingleton<FactureAddedConsumer>(sp =>
{
    var factory = sp.GetRequiredService<Func<string, ConsumerConfig>>();
    var config = factory("FactureCreated");
    return new FactureAddedConsumer(
        config,
        sp.GetRequiredService<ILogger<FactureAddedConsumer>>(),
        sp.GetRequiredService<INotificationApplicationService>(),
        sp.GetRequiredService<IServiceScopeFactory>());
});

builder.Services.AddSingleton<RvdCancelledConsumer>(sp =>
{
    var factory = sp.GetRequiredService<Func<string, ConsumerConfig>>();
    var config = factory("RdvCancelled");
    return new RvdCancelledConsumer(
        config,
        sp.GetRequiredService<ILogger<RvdCancelledConsumer>>(),
        sp.GetRequiredService<INotificationApplicationService>(),
        sp.GetRequiredService<IServiceScopeFactory>());
});

builder.Services.AddSingleton<RvdConfirmedConsumer>(sp =>
{
    var factory = sp.GetRequiredService<Func<string, ConsumerConfig>>();
    var config = factory("RdvConfirmed");
    return new RvdConfirmedConsumer(
        config,
        sp.GetRequiredService<ILogger<RvdConfirmedConsumer>>(),
        sp.GetRequiredService<INotificationApplicationService>(),
        sp.GetRequiredService<IServiceScopeFactory>());
});

builder.Services.AddSingleton<RvdUpdatedConsumer>(sp =>
{
    var factory = sp.GetRequiredService<Func<string, ConsumerConfig>>();
    var config = factory("RdvUpdated");
    return new RvdUpdatedConsumer(
        config,
        sp.GetRequiredService<ILogger<RvdUpdatedConsumer>>(),
        sp.GetRequiredService<INotificationApplicationService>(),
        sp.GetRequiredService<IServiceScopeFactory>());
});

builder.Services.AddSingleton<RvdCreatedConsumer>(sp =>
{
    var factory = sp.GetRequiredService<Func<string, ConsumerConfig>>();
    var config = factory("RdvCreated");
    return new RvdCreatedConsumer(
        config,
        sp.GetRequiredService<ILogger<RvdCreatedConsumer>>(),
        sp.GetRequiredService<INotificationApplicationService>(),
        sp.GetRequiredService<IServiceScopeFactory>());
});

builder.Services.AddSingleton<RvdCancelledByDoctorConsumer>(sp =>
{
    var factory = sp.GetRequiredService<Func<string, ConsumerConfig>>();
    var config = factory("RdvCancelledByDoctor");
    return new RvdCancelledByDoctorConsumer(
        config,
        sp.GetRequiredService<ILogger<RvdCancelledByDoctorConsumer>>(),
        sp.GetRequiredService<INotificationApplicationService>(),
        sp.GetRequiredService<IServiceScopeFactory>());
});

// ---------------------------------------------------
// Hébergement des consumers
// ---------------------------------------------------
builder.Services.AddHostedService(sp => sp.GetRequiredService<FactureAddedConsumer>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<RvdCancelledConsumer>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<RvdConfirmedConsumer>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<RvdUpdatedConsumer>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<RvdCreatedConsumer>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<RvdCancelledByDoctorConsumer>());

// ---------------------------------------------------
// Contrôleurs + config JSON
// ---------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // conserve PascalCase dans DTOs
    });

// ---------------------------------------------------
// Swagger / Scalar (documentation API)
// ---------------------------------------------------
builder.Services.AddOpenApi();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys"))
    .SetApplicationName("MyApp");

// ---------------------------------------------------
// CORS
// ---------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ---------------------------------------------------
// Middleware pipeline
// ---------------------------------------------------
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();              // Swagger UI + OpenAPI
    app.MapScalarApiReference();   // Scalar metadata (si utilisé)
    app.ApplyMigrations();         // Extension custom migration EF
}

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseMetricServer();
app.UseHttpMetrics();

app.MapControllers();

app.Run();
