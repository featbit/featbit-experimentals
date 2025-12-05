using EventsCleanupApi.Data;
using EventsCleanupApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EventsCleanupApi.Services;

public class HierarchyService : IHierarchyService
{
    private readonly EventsDbContext _db;

    public HierarchyService(EventsDbContext db)
    {
        _db = db;
    }

    public async Task<WorkspaceHierarchyResponse> GetHierarchyAsync()
    {
        // Load all data sequentially to avoid DbContext concurrent access issues
        var workspaces = await _db.Workspaces.ToListAsync();
        var organizations = await _db.Organizations.ToListAsync();
        var projects = await _db.Projects.ToListAsync();
        var environments = await _db.Environments.ToListAsync();

        // Build lookup dictionaries for efficient grouping
        var envsByProject = environments
            .GroupBy(e => e.ProjectId)
            .ToDictionary(g => g.Key, g => g.Select(e => new EnvironmentDto
            {
                Id = e.Id,
                Name = e.Name,
                Key = e.Key
            }).ToList());

        var projectsByOrg = projects
            .GroupBy(p => p.OrganizationId)
            .ToDictionary(g => g.Key, g => g.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Key = p.Key,
                Environments = envsByProject.GetValueOrDefault(p.Id, [])
            }).ToList());

        var orgsByWorkspace = organizations
            .GroupBy(o => o.WorkspaceId)
            .ToDictionary(g => g.Key, g => g.Select(o => new OrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                Key = o.Key,
                Projects = projectsByOrg.GetValueOrDefault(o.Id, [])
            }).ToList());

        var workspaceDtos = workspaces.Select(w => new WorkspaceDto
        {
            Id = w.Id,
            Name = w.Name,
            Key = w.Key,
            Organizations = orgsByWorkspace.GetValueOrDefault(w.Id, [])
        }).ToList();

        return new WorkspaceHierarchyResponse
        {
            Workspaces = workspaceDtos
        };
    }
}
