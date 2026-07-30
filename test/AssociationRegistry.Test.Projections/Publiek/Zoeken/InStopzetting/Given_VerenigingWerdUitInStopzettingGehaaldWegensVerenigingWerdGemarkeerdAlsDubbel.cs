namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel(
    PubliekZoekenScenarioFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbelScenario> fixture
)
    : PubliekZoekenScenarioClassFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbelScenario>
{
    [Fact]
    public void InStopzetting_Is_True()
    {
        fixture.Result.InStopzetting.Should().BeFalse();
    }
}
