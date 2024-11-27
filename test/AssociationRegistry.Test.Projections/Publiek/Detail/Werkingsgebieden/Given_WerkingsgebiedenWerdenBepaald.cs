namespace AssociationRegistry.Test.Projections.Publiek.Detail.Werkingsgebieden;

using JsonLdContext;
using Public.Schema.Detail;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenBepaald(PubliekDetailScenarioFixture<WerkingsgebiedenWerdenBepaaldScenario> fixture)
    : PubliekDetailScenarioClassFixture<WerkingsgebiedenWerdenBepaaldScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should()
                  .BeEquivalentTo(fixture.Scenario
                                         .WerkingsgebiedenWerdenBepaald
                                         .Werkingsgebieden
                                         .Select(wg => new PubliekVerenigingDetailDocument.Werkingsgebied
                                          {
                                              JsonLdMetadata = new JsonLdMetadata(
                                                  JsonLdType.Werkingsgebied.CreateWithIdValues(wg.Code),
                                                  JsonLdType.Werkingsgebied.Type),
                                              Code = wg.Code,
                                              Naam = wg.Naam,
                                          }));
}
