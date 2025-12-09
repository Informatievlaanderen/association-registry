namespace AssociationRegistry.MartenDb.Transformers;

using DecentraalBeheer.Vereniging;
using Events;
using Persoonsgegevens;
using Store;

public class KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenTransformer : IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden);
    public Type PersistedEventType => typeof(KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string aggregateId)
    {
        var refId = Guid.NewGuid();
        var originalEvent = (KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden)@event;

        var eventZonderPersoonsgegevens =
            new KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens(refId, originalEvent.VertegenwoordigerId);

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
