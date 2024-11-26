namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using FluentAssertions;
using Framework;
using JsonLdContext;
using Public.Schema.Detail;
using ScenarioClassFixtures;
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
                  .Should()
                  .BeEquivalentTo(
                   [
                       new PubliekVerenigingDetailDocument.Werkingsgebied
                       {
                           JsonLdMetadata = new JsonLdMetadata(
                               JsonLdType.Werkingsgebied.CreateWithIdValues(Werkingsgebied.NietVanToepassing.Code),
                               JsonLdType.Werkingsgebied.Type),

                           Code = Werkingsgebied.NietVanToepassing.Code,
                           Naam = Werkingsgebied.NietVanToepassing.Naam,
                       },
                   ]);
}
