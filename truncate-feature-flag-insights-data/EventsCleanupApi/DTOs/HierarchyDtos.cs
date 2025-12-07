namespace EventsCleanupApi.DTOs;

public record WorkspaceHierarchyResponse
{
    public List<WorkspaceDto> Workspaces { get; init; } = [];
}

public record WorkspaceDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Key { get; init; }
    public List<OrganizationDto> Organizations { get; init; } = [];
}

public record OrganizationDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Key { get; init; }
    public List<ProjectDto> Projects { get; init; } = [];
}

public record ProjectDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Key { get; init; }
    public List<EnvironmentDto> Environments { get; init; } = [];
}

public record EnvironmentDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Key { get; init; }
}
