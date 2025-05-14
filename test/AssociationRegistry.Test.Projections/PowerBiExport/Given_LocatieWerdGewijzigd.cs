namespace AssociationRegistry.Test.Projections.PowerBiExport;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdGewijzigd(PowerBiScenarioFixture<LocatieWerdGewijzigdEventsScenario> fixture) : PowerBiScenarioClassFixture<LocatieWerdGewijzigdEventsScenario>
{
    [Fact]
    public void TheLocatieWerdGewijzigd()
    {
        var locatie = fixture.Result.Locaties.SingleOrDefault(x => x.LocatieId == fixture.Scenario.LocatieWerdGewijzigd.Locatie.LocatieId);

        locatie.Should().NotBeNull();
        locatie!.Naam.Should().Be(fixture.Scenario.LocatieWerdGewijzigd.Locatie.Naam);
    }
}
