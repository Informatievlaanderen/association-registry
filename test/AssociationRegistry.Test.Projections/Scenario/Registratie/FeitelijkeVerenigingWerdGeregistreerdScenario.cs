namespace AssociationRegistry.Test.Projections.Scenario.Registratie;

using Events;
using AutoFixture;
using Events.Enriched;

public class FeitelijkeVerenigingWerdGeregistreerdScenario : ScenarioBase
{
    public FeitelijkeVerenigingMetPersoonsgegevensGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }

    public FeitelijkeVerenigingWerdGeregistreerdScenario()
    {
        FeitelijkeVerenigingWerdGeregistreerd =
            AutoFixture.Create<FeitelijkeVerenigingMetPersoonsgegevensGeregistreerd>();
    }

    public override string VCode => FeitelijkeVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(FeitelijkeVerenigingWerdGeregistreerd.VCode,
            FeitelijkeVerenigingWerdGeregistreerd),
    ];
}
