namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using ImTools;
using Store;

public class VerenigingZonderEigenRechtspersoonWerdGeregistreerdTransformer : IPersoonsgegevensEventTransformer
{
    public Type EventType => typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd);
    public Type PersistedEventType =>
        typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens);

    public EventTransformationResult Transform(IEvent @event, string vCode)
    {
        var originalEvent = (VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)@event;
        var extractedPersoonsgegevens = new List<IPersoonsgegevens>();

        var vertegenwoordigersZonderPersoonsgegevens =
            new List<Registratiedata.VertegenwoordigerZonderPersoonsgegevens>(originalEvent.Vertegenwoordigers.Length);

        SplitVertegenwoordigersPersoonsgegevens(
            vCode,
            originalEvent,
            vertegenwoordigersZonderPersoonsgegevens,
            extractedPersoonsgegevens
        );

        var bankrekeningnummerZonderPersoonsgegevens =
            new List<Registratiedata.BankrekeningnummerZonderPersoonsgegevens>(
                originalEvent.Bankrekeningnummers.Length
            );

        SplitBankrekeningnummerPersoonsgegevens(
            vCode,
            originalEvent,
            bankrekeningnummerZonderPersoonsgegevens,
            extractedPersoonsgegevens
        );

        var transformedEvent = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens(
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
            originalEvent.HoofdactiviteitenVerenigingsloket,
            bankrekeningnummerZonderPersoonsgegevens.ToArray(),
            originalEvent.DuplicatieInfo
        );

        return new EventTransformationResult(transformedEvent, extractedPersoonsgegevens.ToArray());
    }

    private static void SplitVertegenwoordigersPersoonsgegevens(
        string vCode,
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd originalEvent,
        List<Registratiedata.VertegenwoordigerZonderPersoonsgegevens> vertegenwoordigersZonderPersoonsgegevens,
        List<IPersoonsgegevens> extractedPersoonsgegevens
    )
    {
        foreach (var v in originalEvent.Vertegenwoordigers)
        {
            var refId = Guid.NewGuid();

            vertegenwoordigersZonderPersoonsgegevens.Add(
                new Registratiedata.VertegenwoordigerZonderPersoonsgegevens(refId, v.VertegenwoordigerId, v.IsPrimair)
            );

            extractedPersoonsgegevens.Add(
                new VertegenwoordigerPersoonsgegevens(
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
                    v.SocialMedia
                )
            );
        }
    }

    private static void SplitBankrekeningnummerPersoonsgegevens(
        string vCode,
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd originalEvent,
        List<Registratiedata.BankrekeningnummerZonderPersoonsgegevens> bankrekeningnummerZonderPersoonsgegevens,
        List<IPersoonsgegevens> extractedPersoonsgegevens
    )
    {
        foreach (var b in originalEvent.Bankrekeningnummers)
        {
            var refId = Guid.NewGuid();

            bankrekeningnummerZonderPersoonsgegevens.Add(
                new Registratiedata.BankrekeningnummerZonderPersoonsgegevens(refId, b.BankrekeningnummerId, b.Doel)
            );

            extractedPersoonsgegevens.Add(
                new BankrekeningnummerPersoonsgegevens(
                    refId,
                    VCode.Hydrate(vCode),
                    b.BankrekeningnummerId,
                    b.Iban,
                    b.Titularis
                )
            );
        }
    }
}
