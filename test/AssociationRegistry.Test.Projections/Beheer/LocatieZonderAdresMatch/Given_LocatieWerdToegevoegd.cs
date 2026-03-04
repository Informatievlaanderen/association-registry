namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdToegevoegd(LocatiesZonderAdresMatchScenarioFixture<LocatieWerdToegevoegdScenario> fixture)
    : LocatiesZonderAdresMatchScenarioClassFixture<LocatieWerdToegevoegdScenario>
{
    [Fact]
    public void Then_Locatie_Should_Contain_LocationId()
    {
        var expectedLocatieIds = fixture
            .Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.Select(x => x.LocatieId)
            .Append(fixture.Scenario.LocatieWerdToegevoegd.Locatie.LocatieId);

        fixture.Result.Single().LocatieIds.Should().BeEquivalentTo(expectedLocatieIds);
    }
}
