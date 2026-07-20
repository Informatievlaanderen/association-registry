namespace AssociationRegistry.Test.Projections.Publiek.Detail.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestopt(
    PubliekDetailScenarioFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario> fixture
) : PubliekDetailScenarioClassFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario>
{
    [Fact]
    public void Document_Is_Updated() => fixture.Result.InStopzetting.Should().BeFalse();
}
