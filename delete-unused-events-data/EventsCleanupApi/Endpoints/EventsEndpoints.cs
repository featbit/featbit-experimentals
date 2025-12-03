using EventsCleanupApi.DTOs;
using EventsCleanupApi.Services;

namespace EventsCleanupApi.Endpoints;

public static class EventsEndpoints
{
    public static void MapEventsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events").WithTags("Events Cleanup");

        group.MapGet("/summary", GetSummary)
            .WithName("GetEventsSummary")
            .WithOpenApi();

        group.MapPost("/preview-delete", PreviewDelete)
            .WithName("PreviewDeleteEvents")
            .WithOpenApi();

        group.MapDelete("/", DeleteEvents)
            .WithName("DeleteEvents")
            .WithOpenApi();

        group.MapDelete("/flag-insights", DeleteFlagInsights)
            .WithName("DeleteFlagInsightsEvents")
            .WithOpenApi();

        group.MapDelete("/custom-events", DeleteCustomEvents)
            .WithName("DeleteCustomEvents")
            .WithOpenApi();
    }

    private static async Task<IResult> GetSummary(IEventsCleanupService service)
    {
        var summary = await service.GetSummaryAsync();
        return Results.Ok(summary);
    }

    private static async Task<IResult> PreviewDelete(
        DeleteEventsRequest request,
        IEventsCleanupService service)
    {
        var count = await service.PreviewDeleteAsync(request);
        return Results.Ok(new
        {
            EventsToDelete = count,
            Message = "This is a preview. No events were deleted."
        });
    }

    private static async Task<IResult> DeleteEvents(
        DeleteEventsRequest request,
        IEventsCleanupService service)
    {
        var deletedCount = await service.DeleteEventsAsync(request);
        return Results.Ok(new DeleteEventsResponse
        {
            DeletedCount = deletedCount,
            Message = $"Successfully deleted {deletedCount} events."
        });
    }

    private static async Task<IResult> DeleteFlagInsights(
        DateTime beforeDate,
        IEventsCleanupService service)
    {
        var deletedCount = await service.DeleteFlagInsightsAsync(beforeDate);
        return Results.Ok(new DeleteEventsResponse
        {
            DeletedCount = deletedCount,
            Message = $"Successfully deleted {deletedCount} flag insights events before {beforeDate:yyyy-MM-dd HH:mm:ss}."
        });
    }

    private static async Task<IResult> DeleteCustomEvents(
        DateTime beforeDate,
        IEventsCleanupService service)
    {
        var deletedCount = await service.DeleteCustomEventsAsync(beforeDate);
        return Results.Ok(new DeleteEventsResponse
        {
            DeletedCount = deletedCount,
            Message = $"Successfully deleted {deletedCount} custom events before {beforeDate:yyyy-MM-dd HH:mm:ss}."
        });
    }
}
