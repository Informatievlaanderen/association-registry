namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Werkingsgebieden;

using Scenario.Werkingsgebieden;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietBepaald(
    BeheerZoekenScenarioFixture<WerkingsgebiedenWerdenNietBepaaldScenario> fixture)
    : BeheerZoekenScenarioClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario>
{
    [Fact]
    public void Document_Werkingsgebieden_Werden_Niet_Bepaald()
        => fixture.Result
                  .Werkingsgebieden
                  .Should().BeEmpty();
}
