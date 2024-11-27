namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Werkingsgebieden;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietBepaald(PubliekZoekenScenarioFixture<WerkingsgebiedenWerdenNietBepaaldScenario> fixture)
    : PubliekZoekenScenarioClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should().BeEmpty();
}
