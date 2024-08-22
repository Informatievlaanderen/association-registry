namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

public class FeitelijkeVerenigingWerdGeregistreerdScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;

    public FeitelijkeVerenigingWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
        };
}

public class VerborgenFeitelijkeVerenigingWerdGeregistreerdScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;

    public VerborgenFeitelijkeVerenigingWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            new VerenigingWerdUitgeschrevenUitPubliekeDatastroom(),
        };
}
