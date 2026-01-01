namespace AssociationRegistry.Framework.EventMetadata;

public class EventMetadataCollection
{
    private readonly List<IEventMetadata> _items = new();

    public EventMetadataCollection WithTrace(string? traceId)
    {
        if (!string.IsNullOrEmpty(traceId))
            _items.Add(new TraceMetadata(traceId));

        return this;
    }

    public EventMetadataCollection WithSource(SourceFileMetadata sourceFileMetadata)
    {
        _items.Add(sourceFileMetadata);
        return this;
    }

    public IReadOnlyList<IEventMetadata> Items => _items.AsReadOnly();
}
