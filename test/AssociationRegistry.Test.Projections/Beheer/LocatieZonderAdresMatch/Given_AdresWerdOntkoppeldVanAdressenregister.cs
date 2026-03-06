namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresWerdOntkoppeldVanAdressenregister(
    LocatiesZonderAdresMatchScenarioFixture<AdresWerdOntkoppeldVanAdressenregisterScenario> fixture
) : LocatiesZonderAdresMatchScenarioClassFixture<AdresWerdOntkoppeldVanAdressenregisterScenario>
{
    [Fact]
    public void Then_Locatie_Should_Not_Contain_LocationId()
    {
        var expectedLocatieIds = fixture
            .Scenario.AdresWerdOvergenomenUitAdressenregisterScenario.VerenigingWerdGeregistreerd.Locaties.Select(x =>
                x.LocatieId
            )
            .Where(x => x != fixture.Scenario.AdresWerdOntkoppeldVanAdressenregister.LocatieId);

        fixture.Result.Single().LocatieIds.Should().BeEquivalentTo(expectedLocatieIds);
    }
}
