namespace AssociationRegistry.Test.Projections.Publiek.Detail.Werkingsgebieden;

using JsonLdContext;
using Public.Schema.Detail;
using Scenario.Werkingsgebieden;
using Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing(
    PubliekDetailScenarioFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> fixture)
    : PubliekDetailScenarioClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario>
{
    [Fact]
    public void Document_Is_Updated()
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
