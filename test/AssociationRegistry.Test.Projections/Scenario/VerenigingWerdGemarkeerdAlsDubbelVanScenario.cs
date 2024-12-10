namespace AssociationRegistry.Test.Projections.Scenario;

using AutoFixture;
using Events;

public class VerenigingWerdGemarkeerdAlsDubbelVanScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public VerenigingWerdGermarkeerdAlsDubbelVan VerenigingWerdGermarkeerdAlsDubbelVan { get; set; }

    public VerenigingWerdGemarkeerdAlsDubbelVanScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        VerenigingWerdGermarkeerdAlsDubbelVan = AutoFixture.Create<VerenigingWerdGermarkeerdAlsDubbelVan>() with
        {
            VCode = VerenigingWerdGeregistreerd.VCode,
        };
    }

    public override string VCode => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingWerdGeregistreerd, VerenigingWerdGermarkeerdAlsDubbelVan),
    ];
}
