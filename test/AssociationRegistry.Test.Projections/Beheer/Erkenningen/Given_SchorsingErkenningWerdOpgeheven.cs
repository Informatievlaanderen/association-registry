namespace AssociationRegistry.Test.Projections.Beheer.Erkenningen;

using Admin.Schema.Erkenningen;
using AssociationRegistry.Test.Projections.Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_SchorsingErkenningWerdOpgeheven(
    ErkenningenActivatieScenarioFixture<SchorsingVanErkenningWerdOpgehevenScenario> fixture
) : ErkenningenActivatieScenarioClassFixture<SchorsingVanErkenningWerdOpgehevenScenario>
{
    [Fact]
    public void Then_Erkenning_Is_Removed()
    {
        fixture.Result.Should().BeEquivalentTo([
            new ErkenningDocument
            {
                Id = $"{fixture.Scenario.AggregateId}-{fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId}",
                VCode = fixture.Scenario.AggregateId,
                ErkenningId = fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                Status = fixture.Scenario.SchorsingVanErkenningWerdOpgeheven.Status,
                Startdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                Einddatum = fixture.Scenario.ErkenningWerdGeregistreerd.Einddatum,
            },
        ]);
    }
}
