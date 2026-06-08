namespace AssociationRegistry.Test.Projections.Beheer.Erkenningen;

using AssociationRegistry.Admin.Schema.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Test.Projections.Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerlopen(ErkenningenActivatieScenarioFixture<ErkenningWerdVerlopenScenario> fixture)
    : ErkenningenActivatieScenarioClassFixture<ErkenningWerdVerlopenScenario>
{
    [Fact]
    public void Then_Erkenning_Status_Is_Verlopen()
    {
        fixture
            .Result.Should()
            .BeEquivalentTo([
                new ErkenningDocument
                {
                    Id = $"{fixture.Scenario.AggregateId}-{fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId}",
                    VCode = fixture.Scenario.AggregateId,
                    ErkenningId = fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                    Status = ErkenningStatus.Verlopen.Value,
                    Startdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                    Einddatum = fixture.Scenario.ErkenningWerdGeregistreerd.Einddatum,
                },
            ]);
    }
}
