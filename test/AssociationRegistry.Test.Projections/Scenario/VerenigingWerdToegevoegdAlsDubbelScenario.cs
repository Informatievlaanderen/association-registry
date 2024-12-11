namespace AssociationRegistry.Test.Projections.Scenario;

using AutoFixture;
using Events;

/// <summary>
/// This is exactly the same as VerenigingWerdGemarkeerdAlsDubbelVanScenario, but from the POV of the AuthentiekeVereniging
/// </summary>
public class VerenigingWerdToegevoegdAlsDubbelScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenigingWerdGeregistreerd { get; }
    public FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVerenigingWerdGeregistreerd { get; }
    public VerenigingWerdGermarkeerdAlsDubbelVan VerenigingWerdGermarkeerdAlsDubbelVan { get; set; }
    public VerenigingAanvaardeDubbeleVereniging VerenigingAanvaardeDubbeleVereniging { get; set; }

    public VerenigingWerdToegevoegdAlsDubbelScenario()
    {
        DubbeleVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        AuthentiekeVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        VerenigingWerdGermarkeerdAlsDubbelVan = AutoFixture.Create<VerenigingWerdGermarkeerdAlsDubbelVan>() with
        {
            VCode = DubbeleVerenigingWerdGeregistreerd.VCode,
        };

        VerenigingAanvaardeDubbeleVereniging = AutoFixture.Create<VerenigingAanvaardeDubbeleVereniging>() with
        {
            VCode = AuthentiekeVerenigingWerdGeregistreerd.VCode,
            VCodeDubbeleVereniging = DubbeleVerenigingWerdGeregistreerd.VCode,
        };
    }

    public override string VCode => AuthentiekeVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(DubbeleVerenigingWerdGeregistreerd.VCode, DubbeleVerenigingWerdGeregistreerd, VerenigingWerdGermarkeerdAlsDubbelVan),
        new(AuthentiekeVerenigingWerdGeregistreerd.VCode, AuthentiekeVerenigingWerdGeregistreerd, VerenigingAanvaardeDubbeleVereniging),
    ];
}
