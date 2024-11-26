namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using FluentAssertions;
using Framework;
using JsonLdContext;
using Public.Schema.Detail;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenGewijzigd(WerkingsgebiedenWerdenGewijzigdFixture fixture)
    : IClassFixture<WerkingsgebiedenWerdenGewijzigdFixture>
{
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should()
                  .BeEquivalentTo(fixture.Scenario
                                         .WerkingsgebiedenWerdenGewijzigd
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
