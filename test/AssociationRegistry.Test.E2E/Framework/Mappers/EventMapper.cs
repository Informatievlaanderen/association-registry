namespace AssociationRegistry.Test.E2E.Framework.Mappers;

using Admin.Schema.Persoonsgegevens;
using System;
using System.Linq;
using AssociationRegistry.Events;
using Events.Enriched;

public static class EventMapper
{
    public static (
        FeitelijkeVerenigingWerdGeregistreerd Event,
        VertegenwoordigerPersoonsgegevensDocument[] Documents)
        MapDomainWithPersoonsgegevens(
            this FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens src)
    {
        if (src is null) throw new ArgumentNullException(nameof(src));

        var (vertegenwoordigers, docs) = MapVertegenwoordigersMetDocumenten(
            src.VCode,
            src.Vertegenwoordigers
        );

        var domainEvent = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode: src.VCode,
            Naam: src.Naam,
            KorteNaam: src.KorteNaam,
            KorteBeschrijving: src.KorteBeschrijving,
            Startdatum: src.Startdatum,
            Doelgroep: src.Doelgroep,
            IsUitgeschrevenUitPubliekeDatastroom: src.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens: src.Contactgegevens,
            Locaties: src.Locaties,
            Vertegenwoordigers: vertegenwoordigers,
            HoofdactiviteitenVerenigingsloket: src.HoofdactiviteitenVerenigingsloket
        );

        return (domainEvent, docs);
    }

    public static (
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd Event,
        VertegenwoordigerPersoonsgegevensDocument[] Documents)
        MapDomainWithPersoonsgegevens(
            this VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens src)
    {
        if (src is null) throw new ArgumentNullException(nameof(src));

        var (vertegenwoordigers, docs) = MapVertegenwoordigersMetDocumenten(
            src.VCode,
            src.Vertegenwoordigers
        );

        var domainEvent = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
            VCode: src.VCode,
            Naam: src.Naam,
            KorteNaam: src.KorteNaam,
            KorteBeschrijving: src.KorteBeschrijving,
            Startdatum: src.Startdatum,
            Doelgroep: src.Doelgroep,
            IsUitgeschrevenUitPubliekeDatastroom: src.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens: src.Contactgegevens,
            Locaties: src.Locaties,
            Vertegenwoordigers: vertegenwoordigers,
            HoofdactiviteitenVerenigingsloket: src.HoofdactiviteitenVerenigingsloket,
            DuplicatieInfo: null
        );

        return (domainEvent, docs);
    }

    private static (
        Registratiedata.Vertegenwoordiger[] Vertegenwoordigers,
        VertegenwoordigerPersoonsgegevensDocument[] Documents)
        MapVertegenwoordigersMetDocumenten(
            string vCode,
            EnrichedVertegenwoordiger[] enrichedVertegenwoordigers)
    {
        var eventVertegenwoordigers = new List<Registratiedata.Vertegenwoordiger>();
        var documents = new List<VertegenwoordigerPersoonsgegevensDocument>();

        foreach (var enriched in enrichedVertegenwoordigers)
        {
            var eventVertegenwoordiger = new Registratiedata.Vertegenwoordiger(
                enriched.RefId,
                enriched.VertegenwoordigerId,
                enriched.IsPrimair
            );

            eventVertegenwoordigers.Add(eventVertegenwoordiger);

            var doc = new VertegenwoordigerPersoonsgegevensDocument
            {
                RefId = enriched.RefId,
                VCode = vCode,
                VertegenwoordigerId = enriched.VertegenwoordigerId,
                IsPrimair = enriched.IsPrimair,
                Insz = enriched.VertegenwoordigerPersoonsgegevens.Insz,
                Roepnaam = enriched.VertegenwoordigerPersoonsgegevens.Roepnaam,
                Rol = enriched.VertegenwoordigerPersoonsgegevens.Rol,
                Voornaam = enriched.VertegenwoordigerPersoonsgegevens.Voornaam,
                Achternaam = enriched.VertegenwoordigerPersoonsgegevens.Achternaam,
                Email = enriched.VertegenwoordigerPersoonsgegevens.Email,
                Telefoon = enriched.VertegenwoordigerPersoonsgegevens.Telefoon,
                Mobiel = enriched.VertegenwoordigerPersoonsgegevens.Mobiel,
                SocialMedia = enriched.VertegenwoordigerPersoonsgegevens.SocialMedia,
            };

            documents.Add(doc);
        }

        return (eventVertegenwoordigers.ToArray(), documents.ToArray());
    }
}
