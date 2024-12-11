namespace AssociationRegistry.Test.Projections.Scenario;

using AutoFixture;
using Events;

public class VerenigingWerdGemarkeerdAlsDubbelVanScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenigingWerdGeregistreerd { get; }
    public FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVerenigingWerdGeregistreerd { get; }
    public VerenigingWerdGermarkeerdAlsDubbelVan VerenigingWerdGermarkeerdAlsDubbelVan { get; set; }
    public VerenigingAanvaardeDubbeleVereniging VerenigingAanvaardeDubbeleVereniging { get; set; }

    public VerenigingWerdGemarkeerdAlsDubbelVanScenario()
    {
        DubbeleVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        AuthentiekeVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        VerenigingWerdGermarkeerdAlsDubbelVan = AutoFixture.Create<VerenigingWerdGermarkeerdAlsDubbelVan>() with
        {
            VCode = DubbeleVerenigingWerdGeregistreerd.VCode,
            VCodeAuthentiekeVereniging = AuthentiekeVerenigingWerdGeregistreerd.VCode,
        };

        VerenigingAanvaardeDubbeleVereniging = AutoFixture.Create<VerenigingAanvaardeDubbeleVereniging>() with
        {
            VCode = AuthentiekeVerenigingWerdGeregistreerd.VCode,
            VCodeDubbeleVereniging = DubbeleVerenigingWerdGeregistreerd.VCode,
        };
    }

    public override string VCode => DubbeleVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, DubbeleVerenigingWerdGeregistreerd, VerenigingWerdGermarkeerdAlsDubbelVan),
        new(AuthentiekeVerenigingWerdGeregistreerd.VCode, AuthentiekeVerenigingWerdGeregistreerd, VerenigingAanvaardeDubbeleVereniging),
    ];
}
