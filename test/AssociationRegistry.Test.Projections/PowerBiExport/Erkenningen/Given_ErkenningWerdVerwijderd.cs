namespace AssociationRegistry.Test.Projections.PowerBiExport.Erkenningen;

using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerwijderd(PowerBiScenarioFixture<ErkenningWerdVerwijderdScenario> fixture)
    : PowerBiScenarioClassFixture<ErkenningWerdVerwijderdScenario>
{
    [Fact]
    public void Erkenning_Werd_Geschorst()
    {
        fixture.Result.Erkenningen.Should().BeEmpty();
    }
}
