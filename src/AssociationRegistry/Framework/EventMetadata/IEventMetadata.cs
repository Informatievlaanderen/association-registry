namespace AssociationRegistry.Framework.EventMetadata;

using Marten;

public interface IEventMetadata
{
    void ApplyTo(IDocumentSession session);
}
