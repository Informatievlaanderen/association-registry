namespace AssociationRegistry.Test.Common.AutoFixture;

using AssociationRegistry.Framework;
using JasperFx.Events;
using Marten.Events;
using NodaTime;
using System.Text.Json;

public class TestEvent<T> : IEvent<T> where T : notnull
{
    public static implicit operator TestEvent<T>(T e) => new(e);

    public TestEvent(T data, string ovoNumber = "OVO001001", Instant? instant = null)
    {
        Data = data;
        EventTypeName = nameof(T);
        DotNetTypeName = typeof(T).FullName!;
        SetHeader(MetadataHeaderNames.Initiator, JsonSerializer.SerializeToElement(ovoNumber));
        SetHeader(MetadataHeaderNames.Tijdstip, JsonSerializer.SerializeToElement((instant ?? new Instant()).ToString()));
    }

    /// <summary>The actual event data</summary>
    public T Data { get; set; }

    object IEvent.Data
        => Data;

    public Type EventType
        => typeof(T);

    public string EventTypeName { get; set; }
    public string DotNetTypeName { get; set; }

    public void SetHeader(string key, object value)
    {
        Headers ??= new Dictionary<string, object>();
        Headers[key] = value;
    }

    public object? GetHeader(string key)
    {
        var headers = Headers;
        object? obj = null;

        // ISSUE: explicit non-virtual call
        return (headers != null ? headers.TryGetValue(key, out obj) ? 1 : 0 : 0) == 0 ? null : obj;
    }

    public bool IsArchived { get; set; }
    public string? AggregateTypeName { get; set; }

    /// <summary>
    ///     A reference to the stream that contains
    ///     this event
    /// </summary>
    public Guid StreamId { get; set; }

    /// <summary>
    ///     A reference to the stream if the stream
    ///     identifier mode is AsString
    /// </summary>
    public string? StreamKey { get; set; }

    /// <summary>
    ///     An alternative Guid identifier to identify
    ///     events across databases
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>An event's version position within its event stream</summary>
    public long Version { get; set; }

    /// <summary>A global sequential number identifying the Event</summary>
    public long Sequence { get; set; }

    /// <summary>
    ///     The UTC time that this event was originally captured
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    public string TenantId { get; set; } = "*DEFAULT*";

    /// <summary>Optional metadata describing the causation id</summary>
    public string? CausationId { get; set; }

    /// <summary>Optional metadata describing the correlation id</summary>
    public string? CorrelationId { get; set; }

    /// <summary>This is meant to be lazy created, and can be null</summary>
    public Dictionary<string, object>? Headers { get; set; }

    protected bool Equals(TestEvent<T> other)
        => Id.Equals(other.Id);

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        if (this == obj)
            return true;

        return !(obj.GetType() != GetType()) && Equals((TestEvent<T>)obj);
    }

    public override int GetHashCode()
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        => Id.GetHashCode();

    public string Initiator
        => this.GetHeaderString(MetadataHeaderNames.Initiator);

    public Instant Tijdstip
        => this.GetHeaderInstant(MetadataHeaderNames.Tijdstip);
}
