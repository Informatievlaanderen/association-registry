namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Framework;
using AutoFixture;
using Vereniging;

public class V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999001";
        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            "Feestcommittee Oudenaarde",
            "FOud",
            "Het feestcommittee van Oudenaarde",
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
                new Registratiedata.Locatie(
                    1,
                    "Correspondentie",
                    new Registratiedata.Adres(
                        "Stationsstraat",
                        "1",
                        "B",
                        "1790",
                        "Affligem",
                        "België"),
                    IsPrimair: true,
                    "Correspondentie",
                    new Registratiedata.AdresId(Adresbron.AR.Code, "https://data.vlaanderen.be/id/adres/0")),
                new Registratiedata.Locatie(
                    2,
                    "Activiteiten",
                    Adres: null,
                    IsPrimair: false,
                    "Activiteiten",
                    new Registratiedata.AdresId(Adresbron.AR.Code, "https://data.vlaanderen.be/id/adres/0")),
                new Registratiedata.Locatie(
                    3,
                    "Activiteiten",
                    new Registratiedata.Adres(
                        "Dorpstraat",
                        "1",
                        "B",
                        "1790",
                        "Affligem",
                        "België"),
                    IsPrimair: false,
                    "Activiteiten",
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
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
