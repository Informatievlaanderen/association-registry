namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestopt(
    PubliekZoekenScenarioFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario> fixture
) : PubliekZoekenScenarioClassFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario>
{
    [Fact]
    public void InStopzetting_Is_True()
    {
        fixture.Result.InStopzetting.Should().BeFalse();
    }
}
