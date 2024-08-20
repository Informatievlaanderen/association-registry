namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using global::AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario : CommandhandlerScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = fixture.Create<VCode>();
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };
        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>() with { IsPrimair = true };
        VertegenwoordigerWerdToegevoegd2 = fixture.Create<VertegenwoordigerWerdToegevoegd>();
    }

    public override VCode VCode { get; }
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
