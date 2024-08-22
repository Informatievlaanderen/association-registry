namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;

public class V037_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMinimalFields_ForVerwijderContactgegeven : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd;
    public readonly CommandMetadata Metadata;

    public V037_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMinimalFields_ForVerwijderContactgegeven()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999037";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            KorteNaam = string.Empty,
            Startdatum = null,
        };

        ContactgegevenWerdToegevoegd = fixture.Create<ContactgegevenWerdToegevoegd>() with
        {
            ContactgegevenId = 1,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            ContactgegevenWerdToegevoegd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
