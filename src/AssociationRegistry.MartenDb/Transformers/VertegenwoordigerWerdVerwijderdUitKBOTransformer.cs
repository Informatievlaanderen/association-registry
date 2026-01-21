namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Events;
using Persoonsgegevens;
using Store;

public class VertegenwoordigerWerdVerwijderdUitKBOTransformer: IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(VertegenwoordigerWerdVerwijderdUitKBO);
    public Type PersistedEventType => typeof(VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string vCode)
    {
        var original = (VertegenwoordigerWerdVerwijderdUitKBO)@event;
        var refId = Guid.NewGuid();

        var transformedEvent = new VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens(
            refId,
            original.VertegenwoordigerId);

        var persoonsgegevens = new VertegenwoordigerPersoonsgegevens(
            refId: refId,
            VCode: VCode.Hydrate(vCode),
            vertegenwoordigerId: original.VertegenwoordigerId,
            insz: Insz.Hydrate(original.Insz),
            roepnaam: null,
            rol: null,
            voornaam: original.Voornaam,
            achternaam: original.Achternaam,
            email: null,
            telefoon: null,
            mobiel: null,
            socialMedia: null
        );

        return new EventTransformationResult(transformedEvent, [persoonsgegevens]);
    }
}
