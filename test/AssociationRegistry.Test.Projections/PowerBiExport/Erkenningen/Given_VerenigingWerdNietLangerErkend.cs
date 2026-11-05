namespace AssociationRegistry.Test.Projections.PowerBiExport.Erkenningen;

using Events;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdNietLangerErkend(
    PowerBiScenarioFixture<VerenigingWerdNietLangerErkendScenario> fixture
) : PowerBiScenarioClassFixture<VerenigingWerdNietLangerErkendScenario>
{
    [Fact]
    public void VerenigingWerdErkend_Is_Added_To_Historiek()
    {
        fixture.Result.Historiek.Should().ContainSingle(x => x.EventType == nameof(VerenigingWerdNietLangerErkend));
    }

    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
