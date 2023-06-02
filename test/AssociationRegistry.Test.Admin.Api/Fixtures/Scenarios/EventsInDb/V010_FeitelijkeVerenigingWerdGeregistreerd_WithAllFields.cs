namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Framework;
using AutoFixture;

public class V010_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V010_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999010";
        Naam = "De leuke club";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            Contactgegevens = fixture.CreateMany<Registratiedata.Contactgegeven>().Select(
                (contactgegeven, w) => contactgegeven with
                {
                    IsPrimair = w == 0,
                }
            ).ToArray(),
            Vertegenwoordigers = fixture.CreateMany<Registratiedata.Vertegenwoordiger>().Select(
                (vertegenwoordiger, i) => vertegenwoordiger with
                {
                    IsPrimair = i == 0,
                }).ToArray(),
        };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }

    public DateOnly? Startdatum
        => FeitelijkeVerenigingWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
