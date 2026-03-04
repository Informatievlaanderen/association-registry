namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdGewijzigd(LocatiesZonderAdresMatchScenarioFixture<LocatieWerdGewijzigdScenario> fixture)
    : LocatiesZonderAdresMatchScenarioClassFixture<LocatieWerdGewijzigdScenario>
{
    [Fact]
    public void Then_Locatie_Should_Contain_LocationId()
    {
        var expectedLocatieIds =
            fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.Select(x =>
                x.LocatieId
            );

        fixture.Result.Single().LocatieIds.Should().BeEquivalentTo(expectedLocatieIds);
    }
}
