using EventsCleanupApi.DTOs;

namespace EventsCleanupApi.Services;

public interface IEventsCleanupService
{
    Task<EventsSummary> GetSummaryAsync();

    // By timestamp
    Task<long> PreviewDeleteByTimestampAsync(DeleteByTimestampRequest request);
    Task<int> DeleteByTimestampAsync(DeleteByTimestampRequest request);

    // By environment & timestamp
    Task<long> PreviewDeleteByEnvTimestampAsync(DeleteByEnvTimestampRequest request);
    Task<int> DeleteByEnvTimestampAsync(DeleteByEnvTimestampRequest request);

    // By environment & feature flag key
    Task<long> PreviewDeleteByEnvFlagKeyAsync(DeleteByEnvFlagKeyRequest request);
    Task<int> DeleteByEnvFlagKeyAsync(DeleteByEnvFlagKeyRequest request);

    // By project (needs environment list from external API)
    Task<long> PreviewDeleteByProjectAsync(DeleteByProjectRequest request, IEnumerable<string> envIds);
    Task<int> DeleteByProjectAsync(DeleteByProjectRequest request, IEnumerable<string> envIds);
}
