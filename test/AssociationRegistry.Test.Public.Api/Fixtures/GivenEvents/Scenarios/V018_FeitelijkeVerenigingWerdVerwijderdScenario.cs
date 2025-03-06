namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime;
using System;
using Vereniging;

public class V018_FeitelijkeVerenigingWerdVerwijderdScenario : IScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        VCode: "V0001018",
        Naam: "Feestcommittee Oudenaarde",
        KorteNaam: "FOud",
        KorteBeschrijving: "Het feestcommittee van Oudenaarde",
        DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9)),
        new Registratiedata.Doelgroep(Minimumleeftijd: 20, Maximumleeftijd: 71),
        IsUitgeschrevenUitPubliekeDatastroom: false,
        new[]
        {
            new Registratiedata.Contactgegeven(
                ContactgegevenId: 1,
                Contactgegeventype.Email,
                Waarde: "info@FOud.be",
                Beschrijving: "Algemeen",
                IsPrimair: true),
        },
        new[]
        {
            new(
                LocatieId: 1,
                Locatietype: "Correspondentie",
                IsPrimair: true,
                Naam: "Correspondentie",
                new Registratiedata.Adres(
                    Straatnaam: "Stationsstraat",
                    Huisnummer: "1",
                    Busnummer: "B",
                    Postcode: "1790",
                    Gemeente: "Affligem",
                    Land: "België"),
                new Registratiedata.AdresId(
                    Adresbron.AR,
                    Bronwaarde: "https://data.vlaanderen.be/id/adres/0")),
            new(
                LocatieId: 2,
                Locatietype: "Activiteiten",
                IsPrimair: false,
                Naam: "Activiteiten",
                Adres: null,
                new Registratiedata.AdresId(
                    Adresbron.AR,
                    Bronwaarde: "https://data.vlaanderen.be/id/adres/0")),
            new Registratiedata.Locatie(
                LocatieId: 3,
                Locatietype: "Activiteiten",
                IsPrimair: false,
                Naam: "Activiteiten",
                new Registratiedata.Adres(
                    Straatnaam: "Dorpstraat",
                    Huisnummer: "1",
                    Busnummer: "B",
                    Postcode: "1790",
                    Gemeente: "Affligem",
                    Land: "België"),
                AdresId: null),
        },
        new[]
        {
            new Registratiedata.Vertegenwoordiger(
                VertegenwoordigerId: 1,
                Insz: "01234567890",
                IsPrimair: true,
                Roepnaam: "father",
                Rol: "Leader",
                Voornaam: "Odin",
                Achternaam: "Allfather",
                Email: "asgard@world.tree",
                Telefoon: "",
                Mobiel: "",
                SocialMedia: ""),
        },
        new Registratiedata.HoofdactiviteitVerenigingsloket[]
        {
            new(Code: "BLA", Naam: "Buitengewoon Leuke Afkortingen"),
        });

    public VCode VCode
        => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            new VerenigingWerdVerwijderd(VCode, Reden: "Zomaar"),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001", new Instant(), Guid.NewGuid());
}
