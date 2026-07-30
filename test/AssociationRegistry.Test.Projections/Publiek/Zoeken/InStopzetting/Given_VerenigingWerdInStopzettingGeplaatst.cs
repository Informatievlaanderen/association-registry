namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdInStopzettingGeplaatst(
    PubliekZoekenScenarioFixture<VerenigingWerdInStopzettingGeplaatstScenario> fixture
) : PubliekZoekenScenarioClassFixture<VerenigingWerdInStopzettingGeplaatstScenario>
{
    [Fact]
    public void InStopzetting_Is_True()
    {
        fixture.Result.InStopzetting.Should().BeTrue();
    }
}
