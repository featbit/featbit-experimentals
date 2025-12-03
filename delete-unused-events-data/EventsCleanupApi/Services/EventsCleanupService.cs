using EventsCleanupApi.Data;
using EventsCleanupApi.DTOs;
using EventsCleanupApi.Models;
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

    #region By Timestamp

    public async Task<long> PreviewDeleteByTimestampAsync(DeleteByTimestampRequest request)
    {
        var query = BuildTimestampQuery(request);
        return await query.LongCountAsync();
    }

    public async Task<int> DeleteByTimestampAsync(DeleteByTimestampRequest request)
    {
        var query = BuildTimestampQuery(request);
        return await query.ExecuteDeleteAsync();
    }

    private IQueryable<Event> BuildTimestampQuery(DeleteByTimestampRequest request)
    {
        var query = _db.Events.AsQueryable();

        if (request.BeforeDate.HasValue)
        {
            query = query.Where(e => e.Timestamp < request.BeforeDate.Value);
        }

        if (request.AfterDate.HasValue)
        {
            query = query.Where(e => e.Timestamp >= request.AfterDate.Value);
        }

        if (!string.IsNullOrEmpty(request.EventType))
        {
            query = query.Where(e => e.EventType == request.EventType);
        }

        return query;
    }

    #endregion

    #region By Environment & Timestamp

    public async Task<long> PreviewDeleteByEnvTimestampAsync(DeleteByEnvTimestampRequest request)
    {
        var query = BuildEnvTimestampQuery(request);
        return await query.LongCountAsync();
    }

    public async Task<int> DeleteByEnvTimestampAsync(DeleteByEnvTimestampRequest request)
    {
        var query = BuildEnvTimestampQuery(request);
        return await query.ExecuteDeleteAsync();
    }

    private IQueryable<Event> BuildEnvTimestampQuery(DeleteByEnvTimestampRequest request)
    {
        var query = _db.Events.Where(e => e.EnvId == request.EnvId);

        if (request.BeforeDate.HasValue)
        {
            query = query.Where(e => e.Timestamp < request.BeforeDate.Value);
        }

        if (request.AfterDate.HasValue)
        {
            query = query.Where(e => e.Timestamp >= request.AfterDate.Value);
        }

        if (!string.IsNullOrEmpty(request.EventType))
        {
            query = query.Where(e => e.EventType == request.EventType);
        }

        return query;
    }

    #endregion

    #region By Environment & Feature Flag Key

    public async Task<long> PreviewDeleteByEnvFlagKeyAsync(DeleteByEnvFlagKeyRequest request)
    {
        var query = BuildEnvFlagKeyQuery(request);
        return await query.LongCountAsync();
    }

    public async Task<int> DeleteByEnvFlagKeyAsync(DeleteByEnvFlagKeyRequest request)
    {
        var query = BuildEnvFlagKeyQuery(request);
        return await query.ExecuteDeleteAsync();
    }

    private IQueryable<Event> BuildEnvFlagKeyQuery(DeleteByEnvFlagKeyRequest request)
    {
        // Query using PostgreSQL JSONB operator to filter by featureFlagKey in properties
        return _db.Events
            .Where(e => e.EnvId == request.EnvId)
            .Where(e => e.EventType == "FlagValue")
            .Where(e => EF.Functions.JsonContains(
                e.Properties!,
                $"{{\"featureFlagKey\": \"{request.FeatureFlagKey}\"}}"));
    }

    #endregion

    #region By Project

    public async Task<long> PreviewDeleteByProjectAsync(DeleteByProjectRequest request, IEnumerable<string> envIds)
    {
        var query = BuildProjectQuery(request, envIds);
        return await query.LongCountAsync();
    }

    public async Task<int> DeleteByProjectAsync(DeleteByProjectRequest request, IEnumerable<string> envIds)
    {
        var query = BuildProjectQuery(request, envIds);
        return await query.ExecuteDeleteAsync();
    }

    private IQueryable<Event> BuildProjectQuery(DeleteByProjectRequest request, IEnumerable<string> envIds)
    {
        var envIdList = envIds.ToList();
        var query = _db.Events.Where(e => envIdList.Contains(e.EnvId!));

        if (request.BeforeDate.HasValue)
        {
            query = query.Where(e => e.Timestamp < request.BeforeDate.Value);
        }

        if (request.AfterDate.HasValue)
        {
            query = query.Where(e => e.Timestamp >= request.AfterDate.Value);
        }

        if (!string.IsNullOrEmpty(request.EventType))
        {
            query = query.Where(e => e.EventType == request.EventType);
        }

        return query;
    }

    #endregion
}
