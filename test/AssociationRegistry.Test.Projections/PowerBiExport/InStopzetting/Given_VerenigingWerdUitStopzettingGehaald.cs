namespace AssociationRegistry.Test.Projections.PowerBiExport.InStopzetting;

using Events;
using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitStopzettingGehaald(
    PowerBiScenarioFixture<VerenigingWerdUitStopzettingGehaaldScenario> fixture
) : PowerBiScenarioClassFixture<VerenigingWerdUitStopzettingGehaaldScenario>
{
    [Fact]
    public void Historiek_Is_Updated()
    {
        fixture.Result.Historiek.Last().EventType.Should().Be(nameof(VerenigingWerdUitStopzettingGehaald));
    }

    [Fact]
    public void InStopzetting_Is_True()
    {
        fixture.Result.InStopzetting.Should().BeFalse();
    }
}
