namespace AssociationRegistry.Test.Projections.Publiek.Detail.Werkingsgebieden;

using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging;
using Public.Schema.Detail;
using Scenario.Werkingsgebieden;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing(
    PubliekDetailScenarioFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> fixture)
    : PubliekDetailScenarioClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario>
{
    [Fact]
    public void Document_Werkingsgebieden_Werden_Niet_Van_Toepassing()
        => fixture.Result
                  .Werkingsgebieden
                  .Should()
                  .BeEquivalentTo(
                   [
                       new PubliekVerenigingDetailDocument.Types.Werkingsgebied
                       {
                           JsonLdMetadata = new JsonLdMetadata(
                               JsonLdType.Werkingsgebied.CreateWithIdValues(Werkingsgebied.NietVanToepassing.Code),
                               JsonLdType.Werkingsgebied.Type),

                           Code = Werkingsgebied.NietVanToepassing.Code,
                           Naam = Werkingsgebied.NietVanToepassing.Naam,
                       },
                   ]);
}
