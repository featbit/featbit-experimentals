using EventsCleanupApi.DTOs;

namespace EventsCleanupApi.Services;

public interface IEventsCleanupService
{
    Task<EventsSummary> GetSummaryAsync();
    Task<long> PreviewDeleteAsync(DeleteEventsRequest request);
    Task<int> DeleteEventsAsync(DeleteEventsRequest request);
    Task<int> DeleteFlagInsightsAsync(DateTime beforeDate);
    Task<int> DeleteCustomEventsAsync(DateTime beforeDate);
}
