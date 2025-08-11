namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Werkingsgebieden;

using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging;
using Public.Schema.Detail;
using Public.Schema.Search;
using Scenario.Werkingsgebieden;
using Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing(
    PubliekZoekenScenarioFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> fixture)
    : PubliekZoekenScenarioClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario>
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
