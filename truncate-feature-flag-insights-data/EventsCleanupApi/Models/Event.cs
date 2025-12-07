using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventsCleanupApi.Models;

[Table("events")]
public class Event
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("distinct_id")]
    public string? DistinctId { get; set; }

    [Column("env_id")]
    public string? EnvId { get; set; }

    [Column("event")]
    public string? EventType { get; set; }

    [Column("properties", TypeName = "jsonb")]
    public string? Properties { get; set; }

    [Column("timestamp")]
    public DateTime Timestamp { get; set; }
}
