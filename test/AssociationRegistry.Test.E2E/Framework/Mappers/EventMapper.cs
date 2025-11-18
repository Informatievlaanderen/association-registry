namespace AssociationRegistry.Test.E2E.Framework.Mappers;

using System;
using System.Collections.Generic;
using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Events;
using Events.Enriched;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;

/// <summary>
/// Wrapper around the geregistreerd-event + de persoonsgegevens-documenten.
/// </summary>
public sealed record GeregistreerdeVerenigingMetPersoonsgegevens<TEvent>(
    TEvent GeregistreerdEvent,
    VertegenwoordigerPersoonsgegevensDocument[] PersoonsgegevensDocumenten);

public static class EventMapper
{
    // -------------------------------
    // 1. Factory helpers using Fixture
    // -------------------------------

    /// <summary>
    /// Maakt met AutoFixture een FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens,
    /// mapt die naar FeitelijkeVerenigingWerdGeregistreerd + documenten,
    /// en geeft dat alles terug als een GeregistreerdeVereniging.
    /// </summary>
    public static GeregistreerdeVerenigingMetPersoonsgegevens<FeitelijkeVerenigingWerdGeregistreerd>
        CreateFeitelijkeGeregistreerdMetPersoonsgegevens(string? insz = null)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var metPersoonsgegevens = fixture.Create<FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens>();

        return metPersoonsgegevens.ToGeregistreerdeVereniging(insz);
    }

    /// <summary>
    /// Maakt met AutoFixture een VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens,
    /// mapt die naar VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd + documenten,
    /// en geeft dat alles terug als een GeregistreerdeVereniging.
    /// </summary>
    public static GeregistreerdeVerenigingMetPersoonsgegevens<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>
        CreateVzerGeregistreerdMetPersoonsgegevens(string? insz = null)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var metPersoonsgegevens = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens>();

        return metPersoonsgegevens.ToGeregistreerdeVereniging(insz);
    }

    // -------------------------------------------------
    // 2. Extension methods: from *MetPersoonsgegevens*
    //    -> (GeregistreerdEvent + docs) wrapper
    // -------------------------------------------------

    public static GeregistreerdeVerenigingMetPersoonsgegevens<FeitelijkeVerenigingWerdGeregistreerd> ToGeregistreerdeVereniging(
        this FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens src,
        string? insz)
    {
        if (src is null) throw new ArgumentNullException(nameof(src));

        if(insz is not null)
            src.Vertegenwoordigers[0] = src.Vertegenwoordigers[0] with{ VertegenwoordigerPersoonsgegevens = src.Vertegenwoordigers[0].VertegenwoordigerPersoonsgegevens with
                {
                    Insz = insz,
                }};

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

        return new GeregistreerdeVerenigingMetPersoonsgegevens<FeitelijkeVerenigingWerdGeregistreerd>(
            domainEvent,
            docs
        );
    }

    public static GeregistreerdeVerenigingMetPersoonsgegevens<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>
        ToGeregistreerdeVereniging(this VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens src, string? insz)
    {
        if (src is null) throw new ArgumentNullException(nameof(src));

        if(insz is not null)
            src.Vertegenwoordigers[0] = src.Vertegenwoordigers[0] with{ VertegenwoordigerPersoonsgegevens = src.Vertegenwoordigers[0].VertegenwoordigerPersoonsgegevens with
            {
                Insz = insz,
            }};


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

        return new GeregistreerdeVerenigingMetPersoonsgegevens<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>(
            domainEvent,
            docs
        );
    }

    // ------------------------------------------
    // 3. Shared mapping for Vertegenwoordigers
    // ------------------------------------------

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
