namespace EventsCleanupApi.DTOs;

public record DeleteEventsRequest
{
    /// <summary>
    /// Delete events before this date
    /// </summary>
    public DateTime? BeforeDate { get; init; }

    /// <summary>
    /// Filter by event type (e.g., "FlagValue" for feature flag insights)
    /// </summary>
    public string? EventType { get; init; }

    /// <summary>
    /// Filter by environment ID
    /// </summary>
    public string? EnvId { get; init; }
}

public record DeleteEventsResponse
{
    public int DeletedCount { get; init; }
    public string Message { get; init; } = string.Empty;
}

public record EventsSummary
{
    public long TotalCount { get; init; }
    public long FlagValueCount { get; init; }
    public long CustomEventsCount { get; init; }
    public DateTime? OldestEventDate { get; init; }
    public DateTime? NewestEventDate { get; init; }
}
