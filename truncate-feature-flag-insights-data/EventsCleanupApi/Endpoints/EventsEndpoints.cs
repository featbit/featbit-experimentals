using EventsCleanupApi.DTOs;
using EventsCleanupApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventsCleanupApi.Endpoints;

public static class EventsEndpoints
{
    public static void MapEventsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/events").WithTags("Events Cleanup");

        // Summary
        group.MapGet("/summary", GetSummary)
            .WithName("GetEventsSummary");

        // By Timestamp
        var byTimestamp = group.MapGroup("/by-timestamp").WithTags("By Timestamp");
        byTimestamp.MapPost("/preview", PreviewDeleteByTimestamp)
            .WithName("PreviewDeleteByTimestamp");
        byTimestamp.MapDelete("/", DeleteByTimestamp)
            .WithName("DeleteByTimestamp");

        // By Environment & Timestamp
        var byEnvTimestamp = group.MapGroup("/by-env-timestamp").WithTags("By Environment & Timestamp");
        byEnvTimestamp.MapPost("/preview", PreviewDeleteByEnvTimestamp)
            .WithName("PreviewDeleteByEnvTimestamp");
        byEnvTimestamp.MapDelete("/", DeleteByEnvTimestamp)
            .WithName("DeleteByEnvTimestamp");

        // By Environment & Feature Flag Key
        var byEnvFlagKey = group.MapGroup("/by-env-flagkey").WithTags("By Environment & Flag Key");
        byEnvFlagKey.MapPost("/preview", PreviewDeleteByEnvFlagKey)
            .WithName("PreviewDeleteByEnvFlagKey");
        byEnvFlagKey.MapDelete("/", DeleteByEnvFlagKey)
            .WithName("DeleteByEnvFlagKey");

        // By Project
        var byProject = group.MapGroup("/by-project").WithTags("By Project");
        byProject.MapPost("/preview", PreviewDeleteByProject)
            .WithName("PreviewDeleteByProject");
        byProject.MapDelete("/", DeleteByProject)
            .WithName("DeleteByProject");
    }

    #region Summary

    private static async Task<IResult> GetSummary(IEventsCleanupService service)
    {
        var summary = await service.GetSummaryAsync();
        return Results.Ok(summary);
    }

    #endregion

    #region By Timestamp

    private static async Task<IResult> PreviewDeleteByTimestamp(
        [FromBody] DeleteByTimestampRequest request,
        IEventsCleanupService service)
    {
        var count = await service.PreviewDeleteByTimestampAsync(request);
        return Results.Ok(new PreviewDeleteResponse { EventsToDelete = count });
    }

    private static async Task<IResult> DeleteByTimestamp(
        [FromBody] DeleteByTimestampRequest request,
        IEventsCleanupService service)
    {
        var deletedCount = await service.DeleteByTimestampAsync(request);
        return Results.Ok(new DeleteEventsResponse
        {
            DeletedCount = deletedCount,
            Message = $"Successfully deleted {deletedCount} events."
        });
    }

    #endregion

    #region By Environment & Timestamp

    private static async Task<IResult> PreviewDeleteByEnvTimestamp(
        [FromBody] DeleteByEnvTimestampRequest request,
        IEventsCleanupService service)
    {
        var count = await service.PreviewDeleteByEnvTimestampAsync(request);
        return Results.Ok(new PreviewDeleteResponse { EventsToDelete = count });
    }

    private static async Task<IResult> DeleteByEnvTimestamp(
        [FromBody] DeleteByEnvTimestampRequest request,
        IEventsCleanupService service)
    {
        var deletedCount = await service.DeleteByEnvTimestampAsync(request);
        return Results.Ok(new DeleteEventsResponse
        {
            DeletedCount = deletedCount,
            Message = $"Successfully deleted {deletedCount} events for environment '{request.EnvId}'."
        });
    }

    #endregion

    #region By Environment & Feature Flag Key

    private static async Task<IResult> PreviewDeleteByEnvFlagKey(
        [FromBody] DeleteByEnvFlagKeyRequest request,
        IEventsCleanupService service)
    {
        var count = await service.PreviewDeleteByEnvFlagKeyAsync(request);
        return Results.Ok(new PreviewDeleteResponse { EventsToDelete = count });
    }

    private static async Task<IResult> DeleteByEnvFlagKey(
        [FromBody] DeleteByEnvFlagKeyRequest request,
        IEventsCleanupService service)
    {
        var deletedCount = await service.DeleteByEnvFlagKeyAsync(request);
        return Results.Ok(new DeleteEventsResponse
        {
            DeletedCount = deletedCount,
            Message = $"Successfully deleted {deletedCount} events for flag '{request.FeatureFlagKey}' in environment '{request.EnvId}'."
        });
    }

    #endregion

    #region By Project

    private static async Task<IResult> PreviewDeleteByProject(
        [FromBody] DeleteByProjectRequest request,
        IEventsCleanupService eventsService,
        IProjectEnvironmentService projectService)
    {
        try
        {
            var envIds = await projectService.GetEnvironmentsByProjectIdAsync(request.ProjectId);
            var count = await eventsService.PreviewDeleteByProjectAsync(request, envIds);
            return Results.Ok(new PreviewDeleteResponse { EventsToDelete = count });
        }
        catch (NotImplementedException ex)
        {
            return Results.BadRequest(new { Error = ex.Message });
        }
    }

    private static async Task<IResult> DeleteByProject(
        [FromBody] DeleteByProjectRequest request,
        IEventsCleanupService eventsService,
        IProjectEnvironmentService projectService)
    {
        try
        {
            var envIds = await projectService.GetEnvironmentsByProjectIdAsync(request.ProjectId);
            var envIdList = envIds.ToList();
            var deletedCount = await eventsService.DeleteByProjectAsync(request, envIdList);
            return Results.Ok(new DeleteEventsResponse
            {
                DeletedCount = deletedCount,
                Message = $"Successfully deleted {deletedCount} events for project '{request.ProjectId}' ({envIdList.Count} environments)."
            });
        }
        catch (NotImplementedException ex)
        {
            return Results.BadRequest(new { Error = ex.Message });
        }
    }

    #endregion
}
