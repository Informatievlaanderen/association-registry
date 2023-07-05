namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Vereniging;

public class V027_LocatieWerdGewijzigd : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly LocatieWerdGewijzigd LocatieWerdGewijzigd;
    public readonly CommandMetadata Metadata;

    public V027_LocatieWerdGewijzigd()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999027";
        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            "Party in Brakeldorp",
            "FBD",
            "De party van Brakeldorp",
            DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9)),
            false,
            Array.Empty<Registratiedata.Contactgegeven>(),
            new[]
            {
                new Registratiedata.Locatie(
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
                    AdresId: new Registratiedata.AdresId(Adresbron.AR.Code, "https://data.vlaanderen.be/id/adres/0")),
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
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Array.Empty< Registratiedata.HoofdactiviteitVerenigingsloket>());

        LocatieWerdGewijzigd = new LocatieWerdGewijzigd(
            new Registratiedata.Locatie(
                1,
                Locatietype.Activiteiten,
                false,
                "Gewijzigd",
                new Registratiedata.Adres(
                    "straatnaam",
                    "99",
                    "A1",
                    "0123",
                    "Zonnedorp",
                    "Frankrijk"),
                new Registratiedata.AdresId(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix+"4")
            ));
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            LocatieWerdGewijzigd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
