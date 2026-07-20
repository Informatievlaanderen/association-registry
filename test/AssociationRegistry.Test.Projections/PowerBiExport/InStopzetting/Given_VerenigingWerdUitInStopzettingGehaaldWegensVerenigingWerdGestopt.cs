namespace AssociationRegistry.Test.Projections.PowerBiExport.InStopzetting;

using Events;
using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestopt(
    PowerBiScenarioFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario> fixture
) : PowerBiScenarioClassFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario>
{
    [Fact]
    public void Historiek_Is_Updated()
    {
        fixture
            .Result.Historiek.Last()
            .EventType.Should()
            .Be(nameof(VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestopt));
    }

    [Fact]
    public void InStopzetting_Is_True()
    {
        fixture.Result.InStopzetting.Should().BeFalse();
    }
}
