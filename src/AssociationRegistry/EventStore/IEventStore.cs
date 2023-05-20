namespace AssociationRegistry.EventStore;

using Framework;

public interface IEventStore
{
    Task<StreamActionResult> Save(string aggregateId, CommandMetadata commandMetadata, params IEvent[] events);

    Task<T> Load<T>(string id) where T : class, IHasVersion;
}
