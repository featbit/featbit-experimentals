using EventsCleanupApi.Data;
using Microsoft.EntityFrameworkCore;

namespace EventsCleanupApi.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/health").WithTags("Health");

        group.MapGet("/", GetHealth)
            .WithName("GetHealth")
            .WithDescription("Check the overall health status of the API");

        group.MapGet("/database", GetDatabaseHealth)
            .WithName("GetDatabaseHealth")
            .WithDescription("Check if the PostgreSQL database connection is healthy");
    }

    private static IResult GetHealth()
    {
        return Results.Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        });
    }

    private static async Task<IResult> GetDatabaseHealth(EventsDbContext db)
    {
        try
        {
            // Try to open a connection and execute a simple query
            var canConnect = await db.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                return Results.Ok(new
                {
                    Status = "Unhealthy",
                    Database = "PostgreSQL",
                    Message = "Cannot connect to database",
                    Timestamp = DateTime.UtcNow
                });
            }

            // Execute a simple query to verify the connection works
            var eventCount = await db.Events.CountAsync();
            
            return Results.Ok(new
            {
                Status = "Healthy",
                Database = "PostgreSQL",
                Message = "Database connection successful",
                EventCount = eventCount,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return Results.Ok(new
            {
                Status = "Unhealthy",
                Database = "PostgreSQL",
                Message = ex.Message,
                Error = ex.GetType().Name,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
