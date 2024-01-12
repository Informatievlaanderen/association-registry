namespace AssociationRegistry.EventStore;

using Framework;
using Vereniging;

public interface IEventStore
{
    Task<StreamActionResult> Save(
        string aggregateId,
        CommandMetadata commandMetadata,
        CancellationToken cancellationToken,
        params IEvent[] events);

    Task<T> Load<T>(string id) where T : class, IHasVersion, new();
    Task<T?> Load<T>(KboNummer kboNummer) where T : class, IHasVersion, new();
}
