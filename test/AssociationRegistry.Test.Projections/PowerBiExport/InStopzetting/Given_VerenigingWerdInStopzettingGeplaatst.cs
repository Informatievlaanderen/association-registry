namespace AssociationRegistry.Test.Projections.PowerBiExport.InStopzetting;

using Events;
using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdInStopzettingGeplaatst(
    PowerBiScenarioFixture<VerenigingWerdInStopzettingGeplaatstScenario> fixture
) : PowerBiScenarioClassFixture<VerenigingWerdInStopzettingGeplaatstScenario>
{
    [Fact]
    public void Historiek_Is_Updated()
    {
        fixture.Result.Historiek.Last().EventType.Should().Be(nameof(VerenigingWerdInStopzettingGeplaatst));
    }

    [Fact]
    public void InStopzetting_Is_True()
    {
        fixture.Result.InStopzetting.Should().BeTrue();
    }
}
