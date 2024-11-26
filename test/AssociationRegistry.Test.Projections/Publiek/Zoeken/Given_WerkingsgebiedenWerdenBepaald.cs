namespace AssociationRegistry.Test.Projections.Publiek.Zoeken;

using FluentAssertions;
using Framework;
using JsonLdContext;
using Public.Schema.Detail;
using Public.Schema.Search;
using Xunit;

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
