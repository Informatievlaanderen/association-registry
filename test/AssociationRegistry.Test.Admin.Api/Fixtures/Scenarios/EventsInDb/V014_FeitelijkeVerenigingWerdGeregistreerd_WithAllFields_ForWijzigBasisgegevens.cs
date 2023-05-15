namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;

public class V014_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_ForWijzigBasisgegevens : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V014_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_ForWijzigBasisgegevens()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999014";
        Naam = "Dee coolste club";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            Contactgegevens = fixture.CreateMany<FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven>().Select(
                (contactgegeven, w) => contactgegeven with
                {
                    IsPrimair = w == 0,
                }
            ).ToArray(),
            Vertegenwoordigers = fixture.CreateMany<FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger>().Select(
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
