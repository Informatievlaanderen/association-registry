namespace AssociationRegistry.Framework.EventMetadata;

using Marten;

public record TraceMetadata(string TraceId) : IEventMetadata
{
    public void ApplyTo(IDocumentSession session)
        => session.SetHeader(MetadataHeaderNames.TraceId, TraceId);
}
