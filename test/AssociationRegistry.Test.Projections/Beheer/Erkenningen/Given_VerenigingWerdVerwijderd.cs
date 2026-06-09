namespace AssociationRegistry.Test.Projections.Beheer.Erkenningen;

using AssociationRegistry.Test.Projections.Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdVerwijderd(
    ErkenningenActivatieScenarioFixture<VerenigingWerdVerwijderdMetErkenningenScenario> fixture
) : ErkenningenActivatieScenarioClassFixture<VerenigingWerdVerwijderdMetErkenningenScenario>
{
    [Fact]
    public void Then_All_Erkenningen_Are_Removed()
    {
        fixture.Result.Should().BeEmpty();
    }
}
