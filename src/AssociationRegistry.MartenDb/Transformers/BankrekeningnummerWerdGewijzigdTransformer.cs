namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Events;
using Persoonsgegevens;
using Store;

public class BankrekeningnummerWerdGewijzigdTransformer: IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(BankrekeningnummerWerdGewijzigd);
    public Type PersistedEventType => typeof(BankrekeningnummerWerdGewijzigdZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string vCode)
    {
        var original = (BankrekeningnummerWerdGewijzigd)@event;
        var refId = Guid.NewGuid();

        var transformedEvent = new BankrekeningnummerWerdGewijzigdZonderPersoonsgegevens(
            refId,
            original.BankrekeningnummerId,
            original.Doel
        );

        var persoonsgegevens = new BankrekeningnummerPersoonsgegevens(
            refId,
            VCode.Hydrate(vCode),
            original.BankrekeningnummerId,
            string.Empty,
            original.Titularis);

        return new EventTransformationResult(transformedEvent, [persoonsgegevens]);
    }
}
