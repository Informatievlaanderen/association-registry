namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresHeeftGeenVerschillenMetAdressenregister(
    LocatiesZonderAdresMatchScenarioFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario> fixture
) : LocatiesZonderAdresMatchScenarioClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario>
{
    [Fact]
    public void Then_Locatie_Should_Contain_LocationId()
    {
        var expectedLocatieIds = fixture
            .Scenario.VerenigingWerdGeregistreerd.Locaties.Select(x => x.LocatieId)
            .Where(x => x != fixture.Scenario.AdresHeeftGeenVerschillenMetAdressenregister.LocatieId);

        fixture.Result.Single().LocatieIds.Should().BeEquivalentTo(expectedLocatieIds);
    }
}
