namespace AssociationRegistry.EventStore;

using Events;
using Framework;
using Marten;
using Vereniging;

public interface IEventStore
{
    Task<StreamActionResult> Save(
        string aggregateId,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events);

    Task<StreamActionResult> Save(
        string aggregateId,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events);

    Task<T> Load<T>(string id, long? expectedVersion) where T : class, IHasVersion, new();
    Task<T?> Load<T>(KboNummer kboNummer, long? expectedVersion) where T : class, IHasVersion, new();
    Task<bool> Exists(VCode vCode);
}
