namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Events;
using Persoonsgegevens;
using Store;

public class BankrekeningnummerWerdGevalideerdTransformer: IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(BankrekeningnummerWerdGevalideerd);
    public Type PersistedEventType => typeof(BankrekeningnummerWerdGevalideerdZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string vCode)
    {
        var original = (BankrekeningnummerWerdGevalideerd)@event;
        var refId = Guid.NewGuid();

        var transformedEvent = new BankrekeningnummerWerdGevalideerdZonderPersoonsgegevens(
            refId,
            original.BankrekeningnummerId,
            original.GevalideerdDoor);

        var persoonsgegevens = new BankrekeningnummerPersoonsgegevens(
            refId,
            VCode.Hydrate(vCode),
            original.BankrekeningnummerId,
            original.Iban,
            original.Titularis);

        return new EventTransformationResult(transformedEvent, [persoonsgegevens]);
    }
}
