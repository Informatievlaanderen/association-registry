namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using EventFactories;
using Events;
using EventStore;
using global::AutoFixture;
using Vereniging;

public class V023_LocatieWerdToegevoegd : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly LocatieWerdToegevoegd LocatieWerdToegevoegd;
    public readonly CommandMetadata Metadata;

    public V023_LocatieWerdToegevoegd()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999023";

        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            Naam: "Party in Brakeldorp",
            KorteNaam: "FBD",
            KorteBeschrijving: "De party van Brakeldorp",
            DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9)),
            EventFactory.Doelgroep(Doelgroep.Null),
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
                new Registratiedata.Locatie(
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
                    new Registratiedata.AdresId(Adresbron.AR.Code, Bronwaarde: "https://data.vlaanderen.be/id/adres/0")),
                new Registratiedata.Locatie(
                    LocatieId: 2,
                    Locatietype: "Activiteiten",
                    IsPrimair: false,
                    Naam: "Activiteiten",
                    Adres: null,
                    new Registratiedata.AdresId(Adresbron.AR.Code, Bronwaarde: "https://data.vlaanderen.be/id/adres/0")),
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

        LocatieWerdToegevoegd = new LocatieWerdToegevoegd(
            new Registratiedata.Locatie(
                LocatieId: 4,
                Locatietype.Activiteiten,
                IsPrimair: false,
                Naam: "Toegevoegd",
                new Registratiedata.Adres(
                    Straatnaam: "straatnaam",
                    Huisnummer: "1",
                    Busnummer: "B",
                    Postcode: "0123",
                    Gemeente: "Zonnedorp",
                    Land: "Frankrijk"),
                new Registratiedata.AdresId(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix + "1")
            ));

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            LocatieWerdToegevoegd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
