namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Store;
using System.Collections.ObjectModel;

public class VertegenwoordigerWerdGewijzigdTransformer : IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(VertegenwoordigerWerdGewijzigd);
    public Type PersistedEventType => typeof(VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string vCode)
    {
        var original = (VertegenwoordigerWerdGewijzigd)@event;
        var refId = Guid.NewGuid();

        var transformedEvent = new VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens(
            refId,
            original.VertegenwoordigerId,
            original.IsPrimair
        );

        var persoonsgegevens = new VertegenwoordigerPersoonsgegevens(
            refId,
            VCode.Hydrate(vCode),
            original.VertegenwoordigerId,
            null,
            original.Roepnaam,
            original.Rol,
            original.Voornaam,
            original.Achternaam,
            original.Email,
            original.Telefoon,
            original.Mobiel,
            original.SocialMedia
        );

        return new EventTransformationResult(transformedEvent, [persoonsgegevens]);
    }
}
