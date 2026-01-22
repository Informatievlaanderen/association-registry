namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Events;
using Persoonsgegevens;
using Store;

public class BankrekeningnummerWerdVerwijderdTransformer: IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(BankrekeningnummerWerdVerwijderd);
    public Type PersistedEventType => typeof(BankrekeningnummerWerdVerwijderdZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string vCode)
    {
        var original = (BankrekeningnummerWerdVerwijderd)@event;
        var refId = Guid.NewGuid();

        var transformedEvent = new BankrekeningnummerWerdVerwijderdZonderPersoonsgegevens(
            refId,
            original.BankrekeningnummerId);

        var persoonsgegevens = new BankrekeningnummerPersoonsgegevens(
            refId,
            VCode.Hydrate(vCode),
            original.BankrekeningnummerId,
            original.Iban,
            string.Empty);

        return new EventTransformationResult(transformedEvent, [persoonsgegevens]);
    }
}
