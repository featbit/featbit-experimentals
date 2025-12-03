using EventsCleanupApi.Data;
using EventsCleanupApi.Endpoints;
using EventsCleanupApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowReactDev");

app.UseHttpsRedirection();

// Map endpoints
app.MapEventsEndpoints();
app.MapHierarchyEndpoints();

app.Run();
