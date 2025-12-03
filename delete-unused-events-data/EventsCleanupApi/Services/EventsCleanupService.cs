using EventsCleanupApi.Data;
using EventsCleanupApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EventsCleanupApi.Services;

public class EventsCleanupService : IEventsCleanupService
{
    private readonly EventsDbContext _db;

    public EventsCleanupService(EventsDbContext db)
    {
        _db = db;
    }

    public async Task<EventsSummary> GetSummaryAsync()
    {
        var totalCount = await _db.Events.LongCountAsync();
        var flagValueCount = await _db.Events.LongCountAsync(e => e.EventType == "FlagValue");
        var customEventsCount = totalCount - flagValueCount;
        var oldestEvent = await _db.Events.OrderBy(e => e.Timestamp).FirstOrDefaultAsync();
        var newestEvent = await _db.Events.OrderByDescending(e => e.Timestamp).FirstOrDefaultAsync();

        return new EventsSummary
        {
            TotalCount = totalCount,
            FlagValueCount = flagValueCount,
            CustomEventsCount = customEventsCount,
            OldestEventDate = oldestEvent?.Timestamp,
            NewestEventDate = newestEvent?.Timestamp
        };
    }

    public async Task<long> PreviewDeleteAsync(DeleteEventsRequest request)
    {
        var query = BuildQuery(request);
        return await query.LongCountAsync();
    }

    public async Task<int> DeleteEventsAsync(DeleteEventsRequest request)
    {
        var query = BuildQuery(request);
        return await query.ExecuteDeleteAsync();
    }

    public async Task<int> DeleteFlagInsightsAsync(DateTime beforeDate)
    {
        return await _db.Events
            .Where(e => e.EventType == "FlagValue" && e.Timestamp < beforeDate)
            .ExecuteDeleteAsync();
    }

    public async Task<int> DeleteCustomEventsAsync(DateTime beforeDate)
    {
        return await _db.Events
            .Where(e => e.EventType != "FlagValue" && e.Timestamp < beforeDate)
            .ExecuteDeleteAsync();
    }

    private IQueryable<Models.Event> BuildQuery(DeleteEventsRequest request)
    {
        var query = _db.Events.AsQueryable();

        if (request.BeforeDate.HasValue)
        {
            query = query.Where(e => e.Timestamp < request.BeforeDate.Value);
        }

        if (!string.IsNullOrEmpty(request.EventType))
        {
            query = query.Where(e => e.EventType == request.EventType);
        }

        if (!string.IsNullOrEmpty(request.EnvId))
        {
            query = query.Where(e => e.EnvId == request.EnvId);
        }

        return query;
    }
}
