namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;

public class V041_FeitelijkeVerenigingWerdGestopt : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdGestopt VerenigingWerdGestopt;
    public readonly EinddatumWerdGewijzigd EinddatumWerdGewijzigd;
    public readonly CommandMetadata Metadata;

    public V041_FeitelijkeVerenigingWerdGestopt()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999041";

        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            "De dulste van Hulste",
            "",
            "",
            null,
            new Registratiedata.Doelgroep(0, 150),
            false,
            Array.Empty<Registratiedata.Contactgegeven>(),
            Array.Empty<Registratiedata.Locatie>(),
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()
        );

        VerenigingWerdGestopt = new VerenigingWerdGestopt(new DateOnly(2023, 09, 06));
        EinddatumWerdGewijzigd = new EinddatumWerdGewijzigd(new DateOnly(1990,01,01));

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd, VerenigingWerdGestopt ,EinddatumWerdGewijzigd};

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
