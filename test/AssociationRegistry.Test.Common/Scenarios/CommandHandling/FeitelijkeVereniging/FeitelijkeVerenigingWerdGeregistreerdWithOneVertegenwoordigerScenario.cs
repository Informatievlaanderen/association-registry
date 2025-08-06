namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using global::AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdWithOneVertegenwoordigerScenario : CommandhandlerScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerdWithOneVertegenwoordigerScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = fixture.Create<VCode>();
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Vertegenwoordigers = new []{fixture.Create<Registratiedata.Vertegenwoordiger>()}
        };

        VertegenwoordigerId = FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.First().VertegenwoordigerId;
    }

    public override VCode VCode { get; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }
    public int VertegenwoordigerId { get; }
    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
        };
    }
}
