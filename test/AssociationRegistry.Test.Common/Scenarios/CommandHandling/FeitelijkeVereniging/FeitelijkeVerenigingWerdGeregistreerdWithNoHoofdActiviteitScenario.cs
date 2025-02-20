namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using global::AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdWithNoHoofdActiviteitScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;

    public FeitelijkeVerenigingWerdGeregistreerdWithNoHoofdActiviteitScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            HoofdactiviteitenVerenigingsloket = Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>(),
        };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
        };
}
