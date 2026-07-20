namespace AssociationRegistry.Test.Projections.PowerBiExport.InStopzetting;

using Events;
using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel(
    PowerBiScenarioFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbelScenario> fixture
) : PowerBiScenarioClassFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbelScenario>
{
    [Fact]
    public void Historiek_Is_Updated()
    {
        fixture
            .Result.Historiek.Last()
            .EventType.Should()
            .Be(nameof(VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel));
    }

    [Fact]
    public void InStopzetting_Is_True()
    {
        fixture.Result.InStopzetting.Should().BeFalse();
    }
}
