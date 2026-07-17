namespace AssociationRegistry.Test.Projections.Publiek.Detail.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdInStopzettingGeplaatst(
    PubliekDetailScenarioFixture<VerenigingWerdInStopzettingGeplaatstScenario> fixture
) : PubliekDetailScenarioClassFixture<VerenigingWerdInStopzettingGeplaatstScenario>
{
    [Fact]
    public void Document_Is_Updated() => fixture.Result.InStopzetting.Should().BeTrue();
}
