namespace AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;

using Events;
using EventStore;
using Framework;
using Marten;

public interface IDubbelDetectieRepository
{
    public Task<StreamActionResult> Save(
        string aggregateId,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events);
    public Task<StreamActionResult> SaveNew(
        string aggregateId,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events);
}
