namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Framework;
using AutoFixture;

public class V012_FeitelijkeVerenigingWerdGeregistreerd_WithVertegenwoordiger_ForWijzigVertegenwoordiger : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V012_FeitelijkeVerenigingWerdGeregistreerd_WithVertegenwoordiger_ForWijzigVertegenwoordiger()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999012";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Vertegenwoordigers = fixture.CreateMany<Registratiedata.Vertegenwoordiger>().Select(
                (vertegenwoordiger, w) => vertegenwoordiger with
                {
                    IsPrimair = w == 0,
                }
            ).ToArray(),
        };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public DateOnly? Startdatum
        => FeitelijkeVerenigingWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
