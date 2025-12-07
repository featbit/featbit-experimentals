using EventsCleanupApi.Data;
using EventsCleanupApi.Endpoints;
using EventsCleanupApi.Services;
using EventsCleanupApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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

// Add CORS for React dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add PostgreSQL DbContext
builder.Services.AddDbContext<EventsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

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

// Map endpoints
app.MapHealthEndpoints();
app.MapEventsEndpoints();
app.MapHierarchyEndpoints();

app.Run();
