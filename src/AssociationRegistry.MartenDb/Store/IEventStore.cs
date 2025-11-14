namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using Marten;

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

    Task<T> Load<T>(string id, long? expectedVersion) where T : class, IHasVersion;
    Task<VerenigingState> Load(string id, long? expectedVersion);
    Task<T?> Load<T>(KboNummer kboNummer, long? expectedVersion) where T : class, IHasVersion;
    Task<bool> Exists(VCode vCode);
    Task<bool> Exists(KboNummer kboNummer);
    Task<StreamActionResult> SaveNew(string aggregateId, CommandMetadata metadata, CancellationToken cancellationToken, IEvent[] events);
    Task<VCode?> GetVCodeForKbo(string kboNummer);
}
