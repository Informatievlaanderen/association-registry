namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Werkingsgebieden;

using Admin.Schema;
using Admin.Schema.Search;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging;
using Scenario.Werkingsgebieden;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing(
    BeheerZoekenScenarioFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> fixture)
    : BeheerZoekenScenarioClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario>
{
    [Fact]
    public void Document_Werkingsgebieden_Werden_Niet_Van_Toepassing()
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
