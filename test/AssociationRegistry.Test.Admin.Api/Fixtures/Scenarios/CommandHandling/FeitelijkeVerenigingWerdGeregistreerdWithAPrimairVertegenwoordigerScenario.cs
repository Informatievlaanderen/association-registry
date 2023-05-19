namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using Events;
using AssociationRegistry.Framework;
using Framework;
using Vereniging;
using AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario : CommandhandlerScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = fixture.Create<VCode>();
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };
        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>() with { IsPrimair = true };
        VertegenwoordigerWerdToegevoegd2 = fixture.Create<VertegenwoordigerWerdToegevoegd>();
    }

    public VCode VCode { get; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd { get; }
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd2 { get; }

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            VertegenwoordigerWerdToegevoegd,
            VertegenwoordigerWerdToegevoegd2,
        };
    }
}