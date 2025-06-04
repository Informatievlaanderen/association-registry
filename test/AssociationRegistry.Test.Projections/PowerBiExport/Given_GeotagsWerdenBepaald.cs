namespace AssociationRegistry.Test.Projections.PowerBiExport;

[Collection(nameof(ProjectionContext))]
public class Given_GeotagsWerdenBepaald(PowerBiScenarioFixture<GeotagsWerdenBepaaldScenario> fixture)
    : PowerBiScenarioClassFixture<GeotagsWerdenBepaaldScenario>
{
    [Fact]
    public void HistoriekIsUpdated()
    {
        fixture.Result.Historiek.Should()
               .ContainSingle(x => x.EventType == "GeotagsWerdenBepaald");
    }
}
