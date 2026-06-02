namespace AssociationRegistry.Test.Projections.Beheer.Erkenningen;

using AssociationRegistry.Admin.Schema.Erkenningen;
using AssociationRegistry.Test.Projections.Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGeregistreerd(
    ErkenningenActivatieScenarioFixture<ErkenningWerdGeregistreerdScenario> fixture
) : ErkenningenActivatieScenarioClassFixture<ErkenningWerdGeregistreerdScenario>
{
    [Fact]
    public void Then_Erkenning_Is_Added()
    {
        fixture.Result.Should().BeEquivalentTo([
                new ErkenningDocument
                {
                    Id = $"{fixture.Scenario.AggregateId}-{fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId}",
                    VCode = fixture.Scenario.AggregateId,
                    ErkenningId = fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                    Startdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                    Einddatum = fixture.Scenario.ErkenningWerdGeregistreerd.Einddatum,
                    Status = fixture.Scenario.ErkenningWerdGeregistreerd.Status,
                },
            ]
        );
    }
}
