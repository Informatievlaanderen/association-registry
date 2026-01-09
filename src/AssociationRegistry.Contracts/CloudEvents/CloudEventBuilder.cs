namespace AssociationRegistry.Contracts.CloudEvents;

using System.Diagnostics;
using CloudNative.CloudEvents;

/// <summary>
/// Fluent builder for creating CloudEvents with distributed tracing for KBO mutations
/// </summary>
public class CloudEventBuilder
{
    private readonly string _eventType;
    private readonly Uri _source;
    private object? _data;
    private string? _sourceFileName;

    private CloudEventBuilder(string eventType, Uri source)
    {
        _eventType = eventType;
        _source = source;
    }

    /// <summary>
    /// Creates a CloudEvent for a mutation file queued for processing
    /// </summary>
    public static CloudEventBuilder MutationFileQueuedForProcessing()
        => new(EventTypes.MutationFileQueuedForProcessing, Sources.FtpProcessor);

    /// <summary>
    /// Creates a CloudEvent for a KBO organisation queued for synchronisation
    /// </summary>
    public static CloudEventBuilder KboSyncOrganisationQueued()
        => new(EventTypes.KboSyncOrganisationQueued, Sources.OrganisationProcessor);

    /// <summary>
    /// Creates a CloudEvent for a KBO function queued for synchronisation
    /// </summary>
    public static CloudEventBuilder KboSyncFunctionQueued()
        => new(EventTypes.KboSyncFunctionQueued, Sources.FunctionProcessor);

    /// <summary>
    /// Creates a CloudEvent for a KBO person queued for synchronisation
    /// </summary>
    public static CloudEventBuilder KboSyncPersonQueued()
        => new(EventTypes.KboSyncPersonQueued, Sources.PersonProcessor);

    /// <summary>
    /// Creates a CloudEvent for manual KBO sync triggered from Admin API
    /// </summary>
    public static CloudEventBuilder ManualKboSyncQueued()
        => new(EventTypes.ManualKboSyncQueued, Sources.AdminApi);

    /// <summary>
    /// Sets the data payload for the CloudEvent
    /// </summary>
    public CloudEventBuilder WithData<T>(T data)
    {
        _data = data;
        return this;
    }

    /// <summary>
    /// Sets the source file name for traceability
    /// </summary>
    public CloudEventBuilder FromFile(string? fileName)
    {
        _sourceFileName = fileName;
        return this;
    }

    /// <summary>
    /// Builds the CloudEvent with W3C distributed tracing context
    /// </summary>
    public CloudEvent Build()
    {
        var cloudEvent = new CloudEvent
        {
            Type = _eventType,
            Source = _source,
            Id = Guid.NewGuid().ToString(),
            Time = DateTimeOffset.UtcNow,
            DataContentType = "application/json",
            Data = _data
        };

        // Add W3C distributed tracing extension attributes
        var currentActivity = Activity.Current;
        if (currentActivity != null)
        {
            cloudEvent["traceparent"] = currentActivity.Id;
            if (!string.IsNullOrEmpty(currentActivity.TraceStateString))
            {
                cloudEvent["tracestate"] = currentActivity.TraceStateString;
            }
        }

        // Add source file name as custom extension attribute
        if (!string.IsNullOrEmpty(_sourceFileName))
        {
            cloudEvent["sourcefilename"] = _sourceFileName;
        }

        return cloudEvent;
    }

    /// <summary>
    /// Builds the CloudEvent and serializes it to JSON
    /// </summary>
    public string BuildAsJson() => Build().ToJson();

    /// <summary>
    /// Well-known event types for KBO mutations
    /// </summary>
    public static class EventTypes
    {
        public const string MutationFileQueuedForProcessing = "com.verenigingen.kbo.mutation.file.queued";
        public const string KboSyncOrganisationQueued = "com.verenigingen.kbo.sync.organisation.queued";
        public const string KboSyncFunctionQueued = "com.verenigingen.kbo.sync.function.queued";
        public const string KboSyncPersonQueued = "com.verenigingen.kbo.sync.person.queued";
        public const string ManualKboSyncQueued = "com.verenigingen.kbo.sync.manual.queued";
    }

    /// <summary>
    /// Well-known sources for KBO CloudEvents
    /// </summary>
    public static class Sources
    {
        public static readonly Uri FtpProcessor = new("urn:kbo:mutations:ftp");
        public static readonly Uri OrganisationProcessor = new("urn:kbo:mutations:organisation");
        public static readonly Uri FunctionProcessor = new("urn:kbo:mutations:function");
        public static readonly Uri PersonProcessor = new("urn:kbo:mutations:person");
        public static readonly Uri AdminApi = new("urn:admin-api:kbo-sync");
    }
}
