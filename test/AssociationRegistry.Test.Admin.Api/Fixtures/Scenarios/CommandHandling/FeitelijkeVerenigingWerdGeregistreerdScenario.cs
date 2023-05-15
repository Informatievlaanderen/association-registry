namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdScenario : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;

    public FeitelijkeVerenigingWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
        };
}