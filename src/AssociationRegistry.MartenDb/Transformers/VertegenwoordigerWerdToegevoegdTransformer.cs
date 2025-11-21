namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Events;
using Persoonsgegevens;
using Store;

public class VertegenwoordigerWerdToegevoegdTransformer : IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(VertegenwoordigerWerdToegevoegd);
    public Type PersistedEventType => typeof(VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string vCode)
    {
        var original = (VertegenwoordigerWerdToegevoegd)@event;
        var refId = Guid.NewGuid();

        var transformedEvent = new VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens(
            refId,
            original.VertegenwoordigerId,
            original.IsPrimair
        );

        var persoonsgegevens = new VertegenwoordigerPersoonsgegevens(
            refId,
            VCode.Hydrate(vCode),
            original.VertegenwoordigerId,
            Insz.Hydrate(original.Insz),
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
