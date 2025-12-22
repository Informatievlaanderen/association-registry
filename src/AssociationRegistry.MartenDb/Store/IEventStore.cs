namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using Persoonsgegevens;

public interface IEventStore
{
    Task<StreamActionResult> Save(
        string aggregateId,
        long aggregateVersion,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events);

    Task<T> Load<T>(string id, long? expectedVersion) where T : class, IHasVersion, new();
    Task<T?> Load<T>(KboNummer kboNummer, long? expectedVersion) where T : class, IHasVersion, new();
    Task<bool> Exists(VCode vCode);
    Task<bool> Exists(KboNummer kboNummer);
    Task<StreamActionResult> SaveNew(string aggregateId, CommandMetadata metadata, CancellationToken cancellationToken, IEvent[] events);
    Task<VCode?> GetVCodeForKbo(string kboNummer);
}
