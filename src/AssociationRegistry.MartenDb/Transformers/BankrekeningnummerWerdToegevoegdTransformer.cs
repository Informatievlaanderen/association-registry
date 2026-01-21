namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Events;
using Persoonsgegevens;
using Store;

public class BankrekeningnummerWerdToegevoegdTransformer: IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(BankrekeningnummerWerdToegevoegd);
    public Type PersistedEventType => typeof(BankrekeningnummerWerdToegevoegdZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string vCode)
    {
        var original = (BankrekeningnummerWerdToegevoegd)@event;
        var refId = Guid.NewGuid();

        var transformedEvent = new BankrekeningnummerWerdToegevoegdZonderPersoonsgegevens(
            refId,
            original.BankrekeningnummerId,
            original.Doel
        );

        var persoonsgegevens = new BankrekeningnummerPersoonsgegevens(
            refId,
            VCode.Hydrate(vCode),
            original.BankrekeningnummerId,
            original.Iban,
            original.Titularis);

        return new EventTransformationResult(transformedEvent, [persoonsgegevens]);
    }
}
