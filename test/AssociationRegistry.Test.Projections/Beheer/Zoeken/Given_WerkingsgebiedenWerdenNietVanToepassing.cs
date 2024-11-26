namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema;
using Admin.Schema.Search;
using FluentAssertions;
using Framework;
using JsonLdContext;
using Vereniging;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing(WerkingsgebiedenWerdenNietVanToepassingFixture fixture)
    : IClassFixture<WerkingsgebiedenWerdenNietVanToepassingFixture>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should().BeEquivalentTo([
                       new VerenigingZoekDocument.Werkingsgebied
                       {
                           JsonLdMetadata = new JsonLdMetadata(
                               JsonLdType.Werkingsgebied.CreateWithIdValues(Werkingsgebied.NietVanToepassing.Code),
                               JsonLdType.Werkingsgebied.Type),
                           Code = Werkingsgebied.NietVanToepassing.Code,
                           Naam = Werkingsgebied.NietVanToepassing.Naam,
                       },
                   ]);
}
