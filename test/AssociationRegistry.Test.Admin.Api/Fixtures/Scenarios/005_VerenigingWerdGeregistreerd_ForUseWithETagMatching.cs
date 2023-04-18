namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Framework;
using AutoFixture;

public class V005_VerenigingWerdGeregistreerd_ForUseWithETagMatching : IEventsInDbScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V005_VerenigingWerdGeregistreerd_ForUseWithETagMatching()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999005";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            KorteNaam = string.Empty,
            KboNummer = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
            Vertegenwoordigers = Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
        };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
