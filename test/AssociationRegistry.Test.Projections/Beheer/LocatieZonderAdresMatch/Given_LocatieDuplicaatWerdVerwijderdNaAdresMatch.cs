namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch(
    LocatiesZonderAdresMatchScenarioFixture<LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario> fixture
) : LocatiesZonderAdresMatchScenarioClassFixture<LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario>
{
    [Fact]
    public void Then_Locatie_Should_Not_Contain_LocationId()
    {
        var expectedLocatieIds = fixture
            .Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.Select(x => x.LocatieId)
            .Where(x => x != fixture.Scenario.LocatieDuplicaatWerdVerwijderdNaAdresMatch.VerwijderdeLocatieId);

        fixture.Result.Single().LocatieIds.Should().BeEquivalentTo(expectedLocatieIds);
    }
}
