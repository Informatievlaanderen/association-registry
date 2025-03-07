namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Werkingsgebieden;

using Admin.Schema;
using Admin.Schema.Search;
using JsonLdContext;
using Scenario.Werkingsgebieden;
using Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing(
    BeheerZoekenScenarioFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> fixture)
    : BeheerZoekenScenarioClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should().BeEquivalentTo([
                       new VerenigingZoekDocument.Types.Werkingsgebied
                       {
                           JsonLdMetadata = new JsonLdMetadata(
                               JsonLdType.Werkingsgebied.CreateWithIdValues(Werkingsgebied.NietVanToepassing.Code),
                               JsonLdType.Werkingsgebied.Type),
                           Code = Werkingsgebied.NietVanToepassing.Code,
                           Naam = Werkingsgebied.NietVanToepassing.Naam,
                       },
                   ]);
}
