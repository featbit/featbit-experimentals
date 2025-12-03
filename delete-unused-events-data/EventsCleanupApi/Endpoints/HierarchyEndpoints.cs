using EventsCleanupApi.Services;

namespace EventsCleanupApi.Endpoints;

public static class HierarchyEndpoints
{
    public static void MapHierarchyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/hierarchy").WithTags("Hierarchy");

        group.MapGet("/", GetHierarchy)
            .WithName("GetHierarchy")
            .WithDescription("Get all workspaces, organizations, projects, and environments in a hierarchical structure")
            .WithOpenApi();
    }

    private static async Task<IResult> GetHierarchy(IHierarchyService service)
    {
        var hierarchy = await service.GetHierarchyAsync();
        return Results.Ok(hierarchy);
    }
}
