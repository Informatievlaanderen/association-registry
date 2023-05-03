namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;

public class V012_VerenigingWerdGeregistreerd_WithVertegenwoordiger : IEventsInDbScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V012_VerenigingWerdGeregistreerd_WithVertegenwoordiger()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999012";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Vertegenwoordigers = fixture.CreateMany<VerenigingWerdGeregistreerd.Vertegenwoordiger>().Select(
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
        => VerenigingWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
