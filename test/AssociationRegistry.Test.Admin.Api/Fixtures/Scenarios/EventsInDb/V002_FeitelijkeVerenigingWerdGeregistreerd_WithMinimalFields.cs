namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;

public class V002_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V002_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999002";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Locatie>(),
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven>(),
            Vertegenwoordigers = Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>(),
        };
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
