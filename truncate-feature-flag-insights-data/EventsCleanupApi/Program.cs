using EventsCleanupApi.Data;
using EventsCleanupApi.Endpoints;
using EventsCleanupApi.Services;
using EventsCleanupApi.Infrastructure;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (OpenTelemetry, health checks, service discovery, resilience)
builder.AddServiceDefaults();

// Configure JSON options for DateTime handling
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    // Add custom DateTime converter to handle multiple formats including PostgreSQL timestamps
    options.SerializerOptions.Converters.Add(new FlexibleNullableDateTimeConverter());
    options.SerializerOptions.Converters.Add(new FlexibleDateTimeConverter());
    options.SerializerOptions.WriteIndented = true;
});

// Add services to the container.
builder.Services.AddOpenApi();

// Add CORS for React dev server and Aspire dashboard
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
    {
        policy.SetIsOriginAllowed(origin => 
            {
                var uri = new Uri(origin);
                // Allow localhost with any port for development
                return uri.Host == "localhost" || uri.Host == "127.0.0.1";
            })
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add PostgreSQL DbContext using Aspire integration
// Connection string comes from appsettings.json -> ConnectionStrings.PostgreSQL
builder.AddNpgsqlDbContext<EventsDbContext>("PostgreSQL");

// Add application services
builder.Services.AddScoped<IEventsCleanupService, EventsCleanupService>();
builder.Services.AddScoped<IProjectEnvironmentService, ProjectEnvironmentService>();
builder.Services.AddScoped<IHierarchyService, HierarchyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Events Cleanup API")
            .WithTheme(ScalarTheme.Purple);
    });
    
    // Redirect root to Scalar documentation
    app.MapGet("/", () => Results.Redirect("/scalar/v1"))
        .ExcludeFromDescription();
}

// Enable CORS
app.UseCors("AllowReactDev");

app.UseHttpsRedirection();

// Map Aspire default endpoints (health checks, etc.)
app.MapDefaultEndpoints();

// Map endpoints
app.MapHealthEndpoints();
app.MapEventsEndpoints();
app.MapHierarchyEndpoints();

app.Run();
