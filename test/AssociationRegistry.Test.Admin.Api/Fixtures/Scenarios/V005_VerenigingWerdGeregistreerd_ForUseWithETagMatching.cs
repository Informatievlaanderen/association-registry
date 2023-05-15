namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Framework;
using AutoFixture;

public class V005_VerenigingWerdGeregistreerd_ForUseWithETagMatching : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V005_VerenigingWerdGeregistreerd_ForUseWithETagMatching()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999005";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Locatie>(),
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven>(),
            Vertegenwoordigers = Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger>(),
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
