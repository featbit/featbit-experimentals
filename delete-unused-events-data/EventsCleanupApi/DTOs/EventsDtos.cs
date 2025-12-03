namespace EventsCleanupApi.DTOs;

/// <summary>
/// Delete events by timestamp range
/// </summary>
public record DeleteByTimestampRequest
{
    /// <summary>
    /// Delete events before this date (exclusive)
    /// </summary>
    public DateTime? BeforeDate { get; init; }

    /// <summary>
    /// Delete events after this date (inclusive) - used with BeforeDate for range
    /// </summary>
    public DateTime? AfterDate { get; init; }

    /// <summary>
    /// Filter by event type (e.g., "FlagValue" for feature flag insights)
    /// </summary>
    public string? EventType { get; init; }
}

/// <summary>
/// Delete events by environment and timestamp
/// </summary>
public record DeleteByEnvTimestampRequest
{
    /// <summary>
    /// Environment ID (required)
    /// </summary>
    public required string EnvId { get; init; }

    /// <summary>
    /// Delete events before this date (exclusive)
    /// </summary>
    public DateTime? BeforeDate { get; init; }

    /// <summary>
    /// Delete events after this date (inclusive) - used with BeforeDate for range
    /// </summary>
    public DateTime? AfterDate { get; init; }

    /// <summary>
    /// Filter by event type (e.g., "FlagValue" for feature flag insights)
    /// </summary>
    public string? EventType { get; init; }
}

/// <summary>
/// Delete events by environment and feature flag key
/// </summary>
public record DeleteByEnvFlagKeyRequest
{
    /// <summary>
    /// Environment ID (required)
    /// </summary>
    public required string EnvId { get; init; }

    /// <summary>
    /// Feature flag key (required) - stored in properties->featureFlagKey
    /// </summary>
    public required string FeatureFlagKey { get; init; }
}

/// <summary>
/// Delete events by project (gets all environments from the project)
/// </summary>
public record DeleteByProjectRequest
{
    /// <summary>
    /// Project ID (required)
    /// </summary>
    public required string ProjectId { get; init; }

    /// <summary>
    /// Delete events before this date (exclusive)
    /// </summary>
    public DateTime? BeforeDate { get; init; }

    /// <summary>
    /// Delete events after this date (inclusive) - used with BeforeDate for range
    /// </summary>
    public DateTime? AfterDate { get; init; }

    /// <summary>
    /// Filter by event type (e.g., "FlagValue" for feature flag insights)
    /// </summary>
    public string? EventType { get; init; }
}

public record DeleteEventsResponse
{
    public int DeletedCount { get; init; }
    public string Message { get; init; } = string.Empty;
}

public record PreviewDeleteResponse
{
    public long EventsToDelete { get; init; }
    public string Message { get; init; } = "This is a preview. No events were deleted.";
}

public record EventsSummary
{
    public long TotalCount { get; init; }
    public long FlagValueCount { get; init; }
    public long CustomEventsCount { get; init; }
    public DateTime? OldestEventDate { get; init; }
    public DateTime? NewestEventDate { get; init; }
}
