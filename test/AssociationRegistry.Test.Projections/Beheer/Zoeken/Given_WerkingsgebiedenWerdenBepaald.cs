namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema;
using Admin.Schema.Search;
using JsonLdContext;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenBepaald(WerkingsgebiedenWerdenBepaaldFixture fixture)
    : IClassFixture<WerkingsgebiedenWerdenBepaaldFixture>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should().BeEquivalentTo(fixture.Scenario.WerkingsgebiedenWerdenBepaald.Werkingsgebieden.Select(
                                               s => new VerenigingZoekDocument.Werkingsgebied
                                               {
                                                   JsonLdMetadata = new JsonLdMetadata(
                                                       JsonLdType.Werkingsgebied.CreateWithIdValues(s.Code),
                                                       JsonLdType.Werkingsgebied.Type),
                                                   Code = s.Code,
                                                   Naam = s.Naam,
                                               }));
}
