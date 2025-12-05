using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventsCleanupApi.Infrastructure;

/// <summary>
/// Custom JSON converter that handles multiple DateTime formats including PostgreSQL timestamp format
/// </summary>
public class FlexibleDateTimeConverter : JsonConverter<DateTime>
{
    private static readonly string[] DateTimeFormats =
    [
        // ISO 8601 formats
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-ddTHH:mm:ss.ffZ",
        "yyyy-MM-ddTHH:mm:ss.fZ",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fff",
        "yyyy-MM-ddTHH:mm:ss.ff",
        "yyyy-MM-ddTHH:mm:ss.f",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-dd",
        // PostgreSQL timestamp format (space-separated)
        "yyyy-MM-dd HH:mm:ss.fff",
        "yyyy-MM-dd HH:mm:ss.ff",
        "yyyy-MM-dd HH:mm:ss.f",
        "yyyy-MM-dd HH:mm:ss",
        // Common alternative formats
        "MM/dd/yyyy HH:mm:ss",
        "MM/dd/yyyy",
        "dd/MM/yyyy",
        "yyyy/MM/dd"
    ];

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        
        if (string.IsNullOrWhiteSpace(dateString))
        {
            throw new JsonException("DateTime value cannot be null or empty");
        }

        // Try parsing with DateTime.Parse first (handles ISO 8601 and most standard formats)
        if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var date))
        {
            return date;
        }

        // Try specific formats
        if (DateTime.TryParseExact(dateString, DateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            return date;
        }

        throw new JsonException($"Unable to parse '{dateString}' as a DateTime. Supported formats include ISO 8601 (e.g., '2024-01-15T10:30:00') and PostgreSQL timestamp (e.g., '2024-01-15 10:30:00.123')");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Write in ISO 8601 format
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture));
    }
}

/// <summary>
/// Custom JSON converter for nullable DateTime that handles multiple formats
/// </summary>
public class FlexibleNullableDateTimeConverter : JsonConverter<DateTime?>
{
    private static readonly FlexibleDateTimeConverter InnerConverter = new();

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return InnerConverter.Read(ref reader, typeof(DateTime), options);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            InnerConverter.Write(writer, value.Value, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
