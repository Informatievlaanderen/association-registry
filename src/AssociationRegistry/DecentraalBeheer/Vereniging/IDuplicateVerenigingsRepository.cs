namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using Marten;

public interface IDuplicateVerenigingsRepository
{
    public Task<StreamActionResult> Save(
        string aggregateId,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events);
}
