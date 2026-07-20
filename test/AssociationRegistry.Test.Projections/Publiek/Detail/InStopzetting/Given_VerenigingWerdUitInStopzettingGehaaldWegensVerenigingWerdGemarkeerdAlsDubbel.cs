namespace AssociationRegistry.Test.Projections.Publiek.Detail.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel(
    PubliekDetailScenarioFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbelScenario> fixture
)
    : PubliekDetailScenarioClassFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbelScenario>
{
    [Fact]
    public void Document_Is_Updated() => fixture.Result.InStopzetting.Should().BeFalse();
}
