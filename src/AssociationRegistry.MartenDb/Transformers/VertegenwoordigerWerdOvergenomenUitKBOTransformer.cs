namespace AssociationRegistry.MartenDb.Transformers;

using DecentraalBeheer.Vereniging;
using Events;
using Persoonsgegevens;
using Store;

public class VertegenwoordigerWerdOvergenomenUitKBOTransformer: IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(VertegenwoordigerWerdOvergenomenUitKBO);
    public Type PersistedEventType => typeof(VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string aggregateId)
    {
        var refId = Guid.NewGuid();
        var originalEvent = (VertegenwoordigerWerdOvergenomenUitKBO)@event;

        var eventZonderPersoonsgegevens =
            new VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevens(refId, originalEvent.VertegenwoordigerId);

        var persoonsgegevens = new VertegenwoordigerPersoonsgegevens(
            refId: refId,
            VCode: VCode.Hydrate(aggregateId),
            vertegenwoordigerId: originalEvent.VertegenwoordigerId,
            insz: Insz.Hydrate(originalEvent.Insz),
            roepnaam: null,
            rol: null,
            voornaam: originalEvent.Voornaam,
            achternaam: originalEvent.Achternaam,
            email: null,
            telefoon: null,
            mobiel: null,
            socialMedia: null
        );

        return new EventTransformationResult(eventZonderPersoonsgegevens, [persoonsgegevens]);
    }
}
