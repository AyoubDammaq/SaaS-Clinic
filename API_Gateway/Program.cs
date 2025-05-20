using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager; // Ajout pour le cache

var builder = WebApplication.CreateBuilder(args);

// Configuration plus robuste
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Ajout de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Ajout du service Ocelot avec cache
builder.Services.AddOcelot(builder.Configuration)
    .AddCacheManager(x =>
    {
        x.WithDictionaryHandle();
    });

// Ajout de Swagger si nécessaire
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("http://localhost:8081/scalar", "Auth API");
        c.SwaggerEndpoint("http://localhost:8083/swagger/v1/swagger.json", "Clinique API");
        c.SwaggerEndpoint("http://localhost:8085/swagger/v1/swagger.json", "Medecin API");
        c.SwaggerEndpoint("http://localhost:8087/swagger/v1/swagger.json", "Patients API");
        c.SwaggerEndpoint("http://localhost:8089/swagger/v1/swagger.json", "RendezVous API");
        c.SwaggerEndpoint("http://localhost:8091/swagger/v1/swagger.json", "Consultation API");
        c.SwaggerEndpoint("http://localhost:8093/swagger/v1/swagger.json", "Facture API");
        c.SwaggerEndpoint("http://localhost:8095/swagger/v1/swagger.json", "Reporting API");
        c.SwaggerEndpoint("http://localhost:8097/swagger/v1/swagger.json", "Notification API");
    });

}

app.UseRouting();
app.UseCors("AllowReactApp");


// Middleware Ocelot
await app.UseOcelot();

app.Run();