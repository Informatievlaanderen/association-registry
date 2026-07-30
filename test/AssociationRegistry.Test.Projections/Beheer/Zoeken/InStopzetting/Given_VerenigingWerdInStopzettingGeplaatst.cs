namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdInStopzettingGeplaatst(
    BeheerZoekenScenarioFixture<VerenigingWerdInStopzettingGeplaatstScenario> fixture
) : BeheerZoekenScenarioClassFixture<VerenigingWerdInStopzettingGeplaatstScenario>
{
    [Fact]
    public void InStopzetting_Is_True()
    {
        fixture.Result.InStopzetting.Should().BeTrue();
    }
}
