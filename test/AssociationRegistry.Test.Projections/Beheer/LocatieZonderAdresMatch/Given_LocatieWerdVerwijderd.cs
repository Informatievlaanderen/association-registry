namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AutoFixture;
using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdVerwijderd(LocatiesZonderAdresMatchScenarioFixture<LocatieWerdVerwijderdScenario> fixture)
    : LocatiesZonderAdresMatchScenarioClassFixture<LocatieWerdVerwijderdScenario>
{
    [Fact]
    public void Then_Locatie_Should_Contain_LocationId()
    {
        var expectedLocatieIds = fixture
            .Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.Select(x => x.LocatieId)
            .Where(x => x != fixture.Scenario.LocatieWerdVerwijderd.Locatie.LocatieId);

        fixture.Result.Single().LocatieIds.Should().BeEquivalentTo(expectedLocatieIds);
    }
}
