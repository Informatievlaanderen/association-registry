namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitStopzettingGehaald(
    BeheerZoekenScenarioFixture<VerenigingWerdUitStopzettingGehaaldScenario> fixture
) : BeheerZoekenScenarioClassFixture<VerenigingWerdUitStopzettingGehaaldScenario>
{
    [Fact]
    public void InStopzetting_Is_True()
    {
        fixture.Result.InStopzetting.Should().BeFalse();
    }
}
