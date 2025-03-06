namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Werkingsgebieden;

using JsonLdContext;
using Public.Schema.Detail;
using Public.Schema.Search;
using Scenario.Werkingsgebieden;
using System.Linq;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenGewijzigd(PubliekZoekenScenarioFixture<WerkingsgebiedenWerdenGewijzigdScenario> fixture)
    : PubliekZoekenScenarioClassFixture<WerkingsgebiedenWerdenGewijzigdScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should().BeEquivalentTo(fixture.Scenario.WerkingsgebiedenWerdenGewijzigd.Werkingsgebieden.Select(
                                               s => new VerenigingZoekDocument.Werkingsgebied
                                               {
                                                   JsonLdMetadata = new JsonLdMetadata(
                                                       JsonLdType.Werkingsgebied.CreateWithIdValues(s.Code),
                                                       JsonLdType.Werkingsgebied.Type),
                                                   Code = s.Code,
                                                   Naam = s.Naam,
                                               }));
}
