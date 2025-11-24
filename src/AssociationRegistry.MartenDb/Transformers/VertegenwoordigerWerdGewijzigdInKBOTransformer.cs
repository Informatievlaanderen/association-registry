namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Store;
using System.Collections.ObjectModel;

public class VertegenwoordigerWerdGewijzigdInKBOTransformer : IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(VertegenwoordigerWerdGewijzigdInKBO);
    public Type PersistedEventType => typeof(VertegenwoordigerWerdGewijzigdInKBOZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string vCode)
    {
        var original = (VertegenwoordigerWerdGewijzigdInKBO)@event;
        var refId = Guid.NewGuid();

        var transformedEvent = new VertegenwoordigerWerdGewijzigdInKBOZonderPersoonsgegevens(
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
