namespace AssociationRegistry.Test.Projections.Scenario.Registratie;

using Events;
using AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }

    public FeitelijkeVerenigingWerdGeregistreerdScenario()
    {
        FeitelijkeVerenigingWerdGeregistreerd =
            AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
    }

    public override string VCode => FeitelijkeVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(FeitelijkeVerenigingWerdGeregistreerd.VCode,
            FeitelijkeVerenigingWerdGeregistreerd),
    ];
}
