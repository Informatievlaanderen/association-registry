namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using Events;
using EventStore;
using global::AutoFixture;
using MartenDb.Store;
using Vereniging;

public class V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VertegenwoordigerPersoonsgegevensDocument VertegenwoordigerPersoonsgegevensDocument;
    public readonly CommandMetadata Metadata;

    public V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999001";
        var refId = Guid.NewGuid();

        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode: VCode,
            Naam: "Feestcommittee Tralalala",
            KorteNaam: "FOud",
            KorteBeschrijving: "Het feestcommittee van Oudenaarde",
            Startdatum: DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9)),
            Doelgroep: new Registratiedata.Doelgroep(Minimumleeftijd: 18, Maximumleeftijd: 90),
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Contactgegevens:
            [
                new Registratiedata.Contactgegeven(
                    ContactgegevenId: 1,
                    Contactgegeventype.Email,
                    Waarde: "info@FOud.be",
                    Beschrijving: "Algemeen",
                    IsPrimair: true),
            ],
            Locaties:
            [
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
            ],
            Vertegenwoordigers:
            [
                new Registratiedata.Vertegenwoordiger(
                    refId,
                    VertegenwoordigerId: 1,
                    IsPrimair: true),
            ],
            HoofdactiviteitenVerenigingsloket:
            [
                new(Code: "BLA", Naam: "Buitengewoon Leuke Afkortingen"),
            ]);

        VertegenwoordigerPersoonsgegevensDocument = fixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
        {
            VCode = VCode,
            RefId = refId,
            VertegenwoordigerId = 1,
            Insz = "01234567890",
            Roepnaam = "father",
            Rol = "Leader",
            Voornaam = "Odin",
            Achternaam = "Allfather",
            Email = "asgard@world.tree",
            Telefoon = "",
            Mobiel = "",
            SocialMedia = "",
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => [FeitelijkeVerenigingWerdGeregistreerd];

    public VertegenwoordigerPersoonsgegevensDocument[] GetVerenwoordigerPersoonsgegevensDocument()
        => [VertegenwoordigerPersoonsgegevensDocument];

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
