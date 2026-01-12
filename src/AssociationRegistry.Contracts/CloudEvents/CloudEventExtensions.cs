namespace AssociationRegistry.Contracts.CloudEvents;

using System.Diagnostics;
using System.Text.Json;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;

public static class CloudEventExtensions
{
    private static readonly JsonEventFormatter JsonFormatter = new(new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    }, new JsonDocumentOptions());

    /// <summary>
    /// Creates a CloudEvent with W3C distributed tracing context from the current Activity
    /// </summary>
    public static CloudEvent CreateWithTracing<T>(
        string type,
        Uri source,
        T data,
        string? sourceFileName = null)
    {
        var cloudEvent = new CloudEvent
        {
            Type = type,
            Source = source,
            Id = Guid.NewGuid().ToString(),
            Time = DateTimeOffset.UtcNow,
            DataContentType = "application/json",
            Data = data
        };

        // Add distributed tracing extension attributes
        var currentActivity = Activity.Current;
        if (currentActivity != null)
        {
            cloudEvent["traceparent"] = currentActivity.Id;
            if (!string.IsNullOrEmpty(currentActivity.TraceStateString))
            {
                cloudEvent["tracestate"] = currentActivity.TraceStateString;
            }
        }

        // Add source file name as custom extension attribute for traceability
        if (!string.IsNullOrEmpty(sourceFileName))
        {
            cloudEvent["sourcefilename"] = sourceFileName;
        }

        return cloudEvent;
    }

    /// <summary>
    /// Extracts the W3C trace context from a CloudEvent and creates a parent ActivityContext
    /// </summary>
    public static ActivityContext? ExtractTraceContext(this CloudEvent cloudEvent)
    {
        var traceparent = cloudEvent["traceparent"]?.ToString();
        if (string.IsNullOrEmpty(traceparent))
        {
            return null;
        }

        var tracestate = cloudEvent["tracestate"]?.ToString();

        if (ActivityContext.TryParse(traceparent, tracestate, out var context))
        {
            return context;
        }

        return null;
    }

    /// <summary>
    /// Gets the source file name extension attribute from a CloudEvent
    /// </summary>
    public static string? GetSourceFileName(this CloudEvent cloudEvent)
    {
        return cloudEvent["sourcefilename"]?.ToString();
    }

    /// <summary>
    /// Serializes a CloudEvent to JSON string for SQS message body
    /// </summary>
    public static string ToJson(this CloudEvent cloudEvent)
    {
        var bytes = JsonFormatter.EncodeStructuredModeMessage(cloudEvent, out _).ToArray();
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Deserializes a CloudEvent from JSON string (SQS message body)
    /// </summary>
    public static CloudEvent? FromJson(string json)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        return JsonFormatter.DecodeStructuredModeMessage(bytes, null, null);
    }
}
