namespace AssociationRegistry.Test.Projections.PowerBiExport.Erkenningen;

using Events;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder(
    PowerBiScenarioFixture<ErkenningOpvolgersWerdenToegevoegdAlsBeheerderScenario> fixture
) : PowerBiScenarioClassFixture<ErkenningOpvolgersWerdenToegevoegdAlsBeheerderScenario>
{
    [Fact]
    public void Historiek_Is_Updated()
    {
        fixture.Result.Historiek.Last().EventType.Should().Be(nameof(ErkenningOpvolgersWerdenToegevoegdAlsBeheerder));
    }
}
