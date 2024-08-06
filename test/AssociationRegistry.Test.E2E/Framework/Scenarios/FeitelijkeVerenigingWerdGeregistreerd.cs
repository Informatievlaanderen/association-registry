namespace AssociationRegistry.Test.E2E.Framework.Scenarios;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Events;
using EventStore;
using TestClasses;
using Vereniging;

public class FeitelijkeVerenigingWerdGeregistreerdScenario: IScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    private VCode _vCode;

    public FeitelijkeVerenigingWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999001";

        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            Naam: "Feestcommittee Oudenaarde",
            KorteNaam: "FOud",
            KorteBeschrijving: "Het feestcommittee van Oudenaarde",
            DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9)),
            new Registratiedata.Doelgroep(Minimumleeftijd: 18, Maximumleeftijd: 90),
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

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public IEvent[] CreateEvents()
        => [FeitelijkeVerenigingWerdGeregistreerd];

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;

    VCode IScenario.VCode
        => _vCode;
}
