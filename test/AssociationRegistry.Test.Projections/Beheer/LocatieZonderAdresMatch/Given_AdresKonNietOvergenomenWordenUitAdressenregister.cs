namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresKonNietOvergenomenWordenUitAdressenregister(
    LocatiesZonderAdresMatchScenarioFixture<AdresKonNietOvergenomenWordenUitAdressenregisterScenario> fixture
) : LocatiesZonderAdresMatchScenarioClassFixture<AdresKonNietOvergenomenWordenUitAdressenregisterScenario>
{
    [Fact]
    public void Then_Locatie_Should_Not_Contain_LocationId()
    {
        var expectedLocatieIds = fixture.Scenario.VerenigingWerdGeregistreerd.Locaties.Select(x => x.LocatieId);

        fixture.Result.Single().LocatieIds.Should().BeEquivalentTo(expectedLocatieIds);
    }
}
