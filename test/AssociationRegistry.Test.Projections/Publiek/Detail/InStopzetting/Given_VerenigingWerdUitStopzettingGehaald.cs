namespace AssociationRegistry.Test.Projections.Publiek.Detail.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitStopzettingGehaald(
    PubliekDetailScenarioFixture<VerenigingWerdUitStopzettingGehaaldScenario> fixture
) : PubliekDetailScenarioClassFixture<VerenigingWerdUitStopzettingGehaaldScenario>
{
    [Fact]
    public void Document_Is_Updated() => fixture.Result.InStopzetting.Should().BeFalse();
}
