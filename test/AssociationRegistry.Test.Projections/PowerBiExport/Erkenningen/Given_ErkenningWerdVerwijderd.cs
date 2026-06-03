namespace AssociationRegistry.Test.Projections.PowerBiExport.Erkenningen;

using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerwijderd(PowerBiScenarioFixture<ErkenningWerdVerwijderdScenario> fixture)
    : PowerBiScenarioClassFixture<ErkenningWerdVerwijderdScenario>
{
    [Fact]
    public void Then_Erkenning_Is_Removed()
    {
        fixture
            .Result.Erkenningen.Should()
            .NotContain(x => x.ErkenningId == fixture.Scenario.ErkenningWerdGeregistreerdToBeRemoved.ErkenningId);

        fixture
            .Result.Erkenningen.Should()
            .ContainSingle(x => x.ErkenningId == fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId);
    }
}
