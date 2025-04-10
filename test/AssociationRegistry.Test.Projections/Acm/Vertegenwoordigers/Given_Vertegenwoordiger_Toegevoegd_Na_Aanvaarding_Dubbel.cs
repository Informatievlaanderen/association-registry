namespace AssociationRegistry.Test.Projections.Acm.Vertegenwoordigers;

using Scenario.Vertegenwoordigers;
using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_Vertegenwoordiger_Toegevoegd_Na_Aanvaarding_Dubbel(
    VerenigingenPerInszScenarioFixture<VertegenwoordigerWerdToegevoegdAanAuthentiekeVerenigingScenario> fixture)
    : VerenigingenPerInszScenarioClassFixture<VertegenwoordigerWerdToegevoegdAanAuthentiekeVerenigingScenario>
{
    [Fact]
    public void Then_Authentieke_Vereniging_Has_Not_DubbeleVereniging_As_CorresponderendeVCodes()
    {
        var authentiekeVereniging =
            fixture.Result.Verenigingen.Single(x => x.VCode == fixture.Scenario.AuthentiekeVerenigingWerdGeregistreerd.VCode);

        authentiekeVereniging.CorresponderendeVCodes.Should().BeEquivalentTo(fixture.Scenario.DubbeleVerenigingWerdGeregistreerd.VCode);
    }
}
