namespace AssociationRegistry.Test.Projections.PowerBiExport.Erkenningen;

using Events;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdErkend(PowerBiScenarioFixture<VerenigingWerdErkendScenario> fixture)
    : PowerBiScenarioClassFixture<VerenigingWerdErkendScenario>
{
    [Fact]
    public void VerenigingWerdErkend_Is_Added_To_Historiek()
    {
        fixture.Result.Historiek.Should().ContainSingle(x => x.EventType == nameof(VerenigingWerdErkend));
    }

    [Fact]
    public void IsErkend_Is_True()
    {
        fixture.Result.IsErkend.Should().BeTrue();
    }
}
