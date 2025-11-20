namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;

public interface IPersoonsgegevensProcessor
{
    Task<IEvent[]> ProcessEvents(string aggregateId, IEvent[] events);
}
