namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;

public class V011_VerenigingWerdGeregistreerd_WithVertegenwoordiger_ForRemovingVertegenwoordiger : IEventsInDbScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V011_VerenigingWerdGeregistreerd_WithVertegenwoordiger_ForRemovingVertegenwoordiger()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999011";
        Naam = "Dee coolste club";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
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
    public string Naam { get; set; }

    public DateOnly? Startdatum
        => VerenigingWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
