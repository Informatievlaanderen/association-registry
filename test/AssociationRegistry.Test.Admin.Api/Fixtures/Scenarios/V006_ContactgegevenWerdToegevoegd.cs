namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Framework;
using AutoFixture;

public class V006_ContactgegevenWerdToegevoegd : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd;
    public readonly CommandMetadata Metadata;

    public V006_ContactgegevenWerdToegevoegd()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999006";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };
        ContactgegevenWerdToegevoegd = fixture.Create<ContactgegevenWerdToegevoegd>() with {ContactgegevenId = 1};
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
