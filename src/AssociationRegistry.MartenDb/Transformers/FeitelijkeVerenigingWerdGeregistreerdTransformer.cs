namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Store;

public class FeitelijkeVerenigingWerdGeregistreerdTransformer : IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(FeitelijkeVerenigingWerdGeregistreerd);
    public Type PersistedEventType => typeof(FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string vCode)
    {
        var originalEvent = (FeitelijkeVerenigingWerdGeregistreerd)@event;
        var vertegenwoordigersZonderPersoonsgegevens =
            new List<Registratiedata.VertegenwoordigerZonderPersoonsgegevens>(
                originalEvent.Vertegenwoordigers.Length);

        List<VertegenwoordigerPersoonsgegevens> vertegenwoordigersPersoonsgegevens = [];
        foreach (var v in originalEvent.Vertegenwoordigers)
        {
            var refId = Guid.NewGuid();

            vertegenwoordigersZonderPersoonsgegevens.Add(
                new Registratiedata.VertegenwoordigerZonderPersoonsgegevens(
                    refId,
                    v.VertegenwoordigerId,
                    v.IsPrimair));

            vertegenwoordigersPersoonsgegevens.Add(new VertegenwoordigerPersoonsgegevens(
                                                       refId,
                                                       VCode.Hydrate(vCode),
                                                       v.VertegenwoordigerId,
                                                       Insz.Hydrate(v.Insz),
                                                       v.Roepnaam,
                                                       v.Rol,
                                                       v.Voornaam,
                                                       v.Achternaam,
                                                       v.Email,
                                                       v.Telefoon,
                                                       v.Mobiel,
                                                       v.SocialMedia));
        }

        var transformedEvent = new FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens(
            originalEvent.VCode,
            originalEvent.Naam,
            originalEvent.KorteNaam,
            originalEvent.KorteBeschrijving,
            originalEvent.Startdatum,
            originalEvent.Doelgroep,
            originalEvent.IsUitgeschrevenUitPubliekeDatastroom,
            originalEvent.Contactgegevens,
            originalEvent.Locaties,
            vertegenwoordigersZonderPersoonsgegevens.ToArray(),
            originalEvent.HoofdactiviteitenVerenigingsloket);

        return new EventTransformationResult(transformedEvent, vertegenwoordigersPersoonsgegevens.ToArray());
    }
}
