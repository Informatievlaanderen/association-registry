namespace AssociationRegistry.Test.Projections.Beheer.Detail.Werkingsgebieden;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using JsonLdContext;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing(
    BeheerDetailScenarioFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> fixture)
    : BeheerDetailScenarioClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should()
                  .BeEquivalentTo(
                   [
                       new Werkingsgebied
                       {
                           JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                               JsonLdType.Werkingsgebied,
                               Vereniging.Werkingsgebied.NietVanToepassing.Code),
                           Code = Vereniging.Werkingsgebied.NietVanToepassing.Code,
                           Naam = Vereniging.Werkingsgebied.NietVanToepassing.Naam,
                       },
                   ]);
}
