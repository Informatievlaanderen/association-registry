namespace AssociationRegistry.EventStore;

using Events;
using Framework;
using Marten;
using Vereniging;

public interface IEventStore
{
    Task<StreamActionResult> Save(
        string aggregateId,
        long aggregateVersion,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events);

    Task<StreamActionResult> Save(
        string aggregateId,
        long aggregateVersion,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events);

    Task<T> Load<T>(string id, long? expectedVersion) where T : class, IHasVersion, new();
    Task<T?> Load<T>(KboNummer kboNummer, long? expectedVersion) where T : class, IHasVersion, new();
    Task<bool> Exists(VCode vCode);
    Task<bool> Exists(KboNummer kboNummer);
    Task<StreamActionResult> SaveNew(VCode aggregateId, long verenigingVersion, IDocumentSession session, CommandMetadata metadata, CancellationToken cancellationToken, IEvent[] events);
}
