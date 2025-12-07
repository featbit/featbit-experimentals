namespace EventsCleanupApi.Services;

/// <summary>
/// Service to get environment IDs from a project.
/// TODO: Implement this service to call external API or database to get environments.
/// </summary>
public interface IProjectEnvironmentService
{
    /// <summary>
    /// Get all environment IDs for a given project.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <returns>List of environment IDs belonging to the project</returns>
    Task<IEnumerable<string>> GetEnvironmentsByProjectIdAsync(string projectId);
}

/// <summary>
/// Placeholder implementation - replace with actual implementation.
/// </summary>
public class ProjectEnvironmentService : IProjectEnvironmentService
{
    public Task<IEnumerable<string>> GetEnvironmentsByProjectIdAsync(string projectId)
    {
        // TODO: Implement this method to:
        // 1. Call FeatBit API to get environments for the project, or
        // 2. Query the database directly if you have access to the environments table
        //
        // Example implementation:
        // var response = await _httpClient.GetAsync($"/api/projects/{projectId}/environments");
        // var environments = await response.Content.ReadFromJsonAsync<List<Environment>>();
        // return environments.Select(e => e.Id);

        throw new NotImplementedException(
            $"GetEnvironmentsByProjectIdAsync is not implemented. " +
            $"Please implement this method to retrieve environment IDs for project '{projectId}'.");
    }
}
