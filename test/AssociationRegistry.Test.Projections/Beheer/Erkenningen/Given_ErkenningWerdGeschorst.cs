namespace AssociationRegistry.Test.Projections.Beheer.Erkenningen;

using AssociationRegistry.Admin.Schema.Erkenningen;
using AssociationRegistry.Test.Projections.Scenario.Erkenningen;
using DecentraalBeheer.Vereniging.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGeschorst(
    ErkenningenActivatieScenarioFixture<ErkenningWerdGeschorstScenario> fixture
) : ErkenningenActivatieScenarioClassFixture<ErkenningWerdGeschorstScenario>
{
    [Fact]
    public void Then_Erkenning_Is_Geschorst()
    {
        fixture.Result.Should().BeEquivalentTo([
            new ErkenningDocument
            {
                Id = $"{fixture.Scenario.AggregateId}-{fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId}",
                VCode = fixture.Scenario.AggregateId,
                ErkenningId = fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                Status = ErkenningStatus.Geschorst.Value,
                Startdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                Einddatum = fixture.Scenario.ErkenningWerdGeregistreerd.Einddatum,
            },
        ]);
    }
}
