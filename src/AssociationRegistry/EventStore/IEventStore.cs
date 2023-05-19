namespace AssociationRegistry.EventStore;

using Framework;

public interface IEventStore
{
    Task<StreamActionResult> Save(string aggregateId, CommandMetadata commandMetadata, CancellationToken cancellationToken, params IEvent[] events);

    Task<T> Load<T>(string id) where T : class, IHasVersion;
}
