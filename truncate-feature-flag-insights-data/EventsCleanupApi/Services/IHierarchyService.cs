using EventsCleanupApi.DTOs;

namespace EventsCleanupApi.Services;

public interface IHierarchyService
{
    Task<WorkspaceHierarchyResponse> GetHierarchyAsync();
}
