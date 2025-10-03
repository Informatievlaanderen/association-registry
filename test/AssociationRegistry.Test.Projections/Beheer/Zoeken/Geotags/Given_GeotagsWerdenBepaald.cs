namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Geotags;

using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.Test.Projections.Beheer.Zoeken;
using AssociationRegistry.Test.Projections.Scenario.Geotags;

[Collection(nameof(ProjectionContext))]
public class Given_GeotagsWerdenBepaald(BeheerZoekenScenarioFixture<GeotagsWerdenBepaaldScenario> fixture)
    : BeheerZoekenScenarioClassFixture<GeotagsWerdenBepaaldScenario>
{
    [Fact]
    public void Geotags_Zijn_Aanwezig()
        => fixture.Result.Geotags.Should()
                  .BeEquivalentTo(
                       fixture.Scenario.GeotagsWerdenBepaald.Geotags.Select(
                           x => new VerenigingZoekDocument.Types.Geotag(x.Identificiatie)));
}
