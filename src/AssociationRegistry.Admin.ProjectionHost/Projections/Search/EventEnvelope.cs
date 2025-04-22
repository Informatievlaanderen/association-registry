namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using Marten.Events;

public class EventEnvelope<T> : IEventEnvelope
{
    public long Sequence
        => Event.Sequence;

    public string VCode
        => Event.StreamKey!;

    public T Data
        => (T)Event.Data;

    public Dictionary<string, object>? Headers
        => Event.Headers;

    public EventEnvelope(IEvent @event)
    {
        Event = @event;
    }

    private IEvent Event { get; }
}
