namespace AssociationRegistry.Test.Projections.Beheer.Erkenningen;

using AssociationRegistry.Admin.Schema.Erkenningen;
using AssociationRegistry.Test.Projections.Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGewijzigd(
    ErkenningenActivatieScenarioFixture<ErkenningWerdGewijzigdScenario> fixture
) : ErkenningenActivatieScenarioClassFixture<ErkenningWerdGewijzigdScenario>
{
    [Fact]
    public void Then_Erkenning_Is_Updated()
    {
        fixture.Result.Should().BeEquivalentTo([
            new ErkenningDocument
            {
                Id = $"{fixture.Scenario.AggregateId}-{fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId}",
                VCode = fixture.Scenario.AggregateId,
                ErkenningId = fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                Status = fixture.Scenario.ErkenningWerdGewijzigd.Status,
                Startdatum = fixture.Scenario.ErkenningWerdGewijzigd.Startdatum,
                Einddatum = fixture.Scenario.ErkenningWerdGewijzigd.Einddatum,
            },
        ]);
    }
}
