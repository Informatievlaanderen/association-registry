namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using Transformers;

public interface IPersoonsgegevensEventTransformer
{
    Type EventType { get; }
    Type PersistedEventType { get; }
    EventTransformationResult Transform(IEvent @event, string aggregateId);
}
