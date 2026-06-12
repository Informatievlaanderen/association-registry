namespace AssociationRegistry.Test.Projections.Publiek.Detail.Werkingsgebieden;

using Scenario.Werkingsgebieden;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietBepaald(PubliekDetailScenarioFixture<WerkingsgebiedenWerdenNietBepaaldScenario> fixture)
    : PubliekDetailScenarioClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario>
{
    [Fact]
    public void Document_Werkingsgebieden_Werden_Niet_Bepaald()
        => fixture.Result
                  .Werkingsgebieden
                  .Should()
                  .BeEmpty();
}
