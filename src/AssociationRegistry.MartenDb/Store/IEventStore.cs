namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.EventStore;
using DecentraalBeheer.Vereniging;
using Events;
using Framework;
using Persoonsgegevens;
using Vereniging;

public interface IEventStore
{
    Task<StreamActionResult> Save(
        string aggregateId,
        long aggregateVersion,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events
    );

    Task<T> Load<T>(string id, long? expectedVersion)
        where T : class, IHasVersion, new();

    Task<T?> Load<T>(KboNummer kboNummer, long? expectedVersion)
        where T : class, IHasVersion, new();

    Task<StreamActionResult> SaveNew(
        string aggregateId,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events
    );

    Task<VCode?> GetVCodeForKbo(string kboNummer);

    Task<StreamActionResult> SaveNewKbo(
        string aggregateId,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events
    );
}
