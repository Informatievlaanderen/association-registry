namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Vereniging;

public class V025_LocatieWerdVerwijderd : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly LocatieWerdVerwijderd LocatieWerdVerwijderd;
    public readonly CommandMetadata Metadata;

    public V025_LocatieWerdVerwijderd()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999025";
        var teVerwijderenLocatie = new Registratiedata.Locatie(
            1,
            "Correspondentie",
            IsPrimair: true,
            Naam: "Correspondentie",
            Adres: new Registratiedata.Adres(
                "Stationsstraat",
                "1",
                "B",
                "1790",
                "Affligem",
                "België"),
            AdresId: new Registratiedata.AdresId(Adresbron.AR.Code, "https://data.vlaanderen.be/id/adres/0"));

        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            "Party in Brakeldorp",
            "FBD",
            "De party van Brakeldorp",
            DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9)),
            false,
            new[]
            {
                new Registratiedata.Contactgegeven(
                    ContactgegevenId: 1,
                    ContactgegevenType.Email,
                    "info@FOud.be",
                    "Algemeen",
                    IsPrimair: true),
            },
            new[]
            {
                teVerwijderenLocatie,
                new Registratiedata.Locatie(
                    2,
                    "Activiteiten",
                    IsPrimair: false,
                    Naam: "Activiteiten",
                    Adres: null,
                    AdresId: new Registratiedata.AdresId(Adresbron.AR.Code, "https://data.vlaanderen.be/id/adres/0")),
                new Registratiedata.Locatie(
                    3,
                    "Activiteiten",
                    IsPrimair: false,
                    Naam: "Activiteiten",
                    Adres: new Registratiedata.Adres(
                        "Dorpstraat",
                        "1",
                        "B",
                        "1790",
                        "Affligem",
                        "België"),
                    AdresId: null),
            },
            new[]
            {
                new Registratiedata.Vertegenwoordiger(
                    VertegenwoordigerId: 1,
                    "01234567890",
                    IsPrimair: true,
                    "father",
                    "Leader",
                    "Odin",
                    "Allfather",
                    "asgard@world.tree",
                    "",
                    "",
                    ""),
            },
            new Registratiedata.HoofdactiviteitVerenigingsloket[]
            {
                new("BLA", "Buitengewoon Leuke Afkortingen"),
            });

        LocatieWerdVerwijderd = new LocatieWerdVerwijderd(teVerwijderenLocatie);
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            LocatieWerdVerwijderd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
