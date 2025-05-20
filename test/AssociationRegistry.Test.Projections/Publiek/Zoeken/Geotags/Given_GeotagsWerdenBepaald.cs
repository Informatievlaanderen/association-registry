namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Geotags;

using Public.Schema.Search;
using Scenario.Geotags;

[Collection(nameof(ProjectionContext))]
public class Given_GeotagsWerdenBepaald(PubliekZoekenScenarioFixture<GeotagsWerdenBepaaldScenario> fixture)
    : PubliekZoekenScenarioClassFixture<GeotagsWerdenBepaaldScenario>
{
    [Fact]
    public void Geotags_Zijn_Aanwezig()
        => fixture.Result.Geotags.Should()
                  .BeEquivalentTo(
                       fixture.Scenario.GeotagsWerdenBepaald.Geotags.Select(
                           x => new VerenigingZoekDocument.Types.Geotag(x.Identificiatie)));
}
