namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using Scenario.Registratie;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
    LocatiesZonderAdresMatchScenarioFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario> fixture
) : LocatiesZonderAdresMatchScenarioClassFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario>
{
    [Fact]
    public void Then_Locaties_Should_Contain_All_LocationId()
    {
        var expectedLocatieIds =
            fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.Select(x =>
                x.LocatieId
            );

        fixture.Result.Single().LocatieIds.Should().BeEquivalentTo(expectedLocatieIds);
    }
}
