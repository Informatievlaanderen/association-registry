namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using global::AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdWithHoofdActiviteitenScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;

    public FeitelijkeVerenigingWerdGeregistreerdWithHoofdActiviteitenScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            HoofdactiviteitenVerenigingsloket = fixture.CreateMany<Registratiedata.HoofdactiviteitVerenigingsloket>().ToArray(),
        };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
        };
}
