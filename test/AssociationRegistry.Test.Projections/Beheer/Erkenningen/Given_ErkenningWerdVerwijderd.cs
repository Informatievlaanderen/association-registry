namespace AssociationRegistry.Test.Projections.Beheer.Erkenningen;

using AssociationRegistry.Test.Projections.Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerwijderd(
    ErkenningenActivatieScenarioFixture<ErkenningWerdVerwijderdScenario> fixture
) : ErkenningenActivatieScenarioClassFixture<ErkenningWerdVerwijderdScenario>
{
    [Fact]
    public void Then_Erkenning_Is_Removed()
    {
        var erkenningIdToBeRemoved =
            $"{fixture.Scenario.AggregateId}-{fixture.Scenario.ErkenningWerdGeregistreerdToBeRemoved.ErkenningId}";

        var erkenningIdNotToBeRemoved =
            $"{fixture.Scenario.AggregateId}-{fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId}";

        fixture.Result.Should().NotContain(x => erkenningIdToBeRemoved == x.Id);
        fixture.Result.Should().Contain(x => erkenningIdNotToBeRemoved == x.Id);
    }
}
