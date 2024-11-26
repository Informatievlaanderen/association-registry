namespace AssociationRegistry.Test.Projections.Publiek.Zoeken;

using Framework;
using Framework.Fixtures;
using Public.Schema.Search;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenGewijzigdFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenGewijzigdScenario,
    VerenigingZoekDocument,
    ProjectionContext>(context)
{
    public override async Task<VerenigingZoekDocument> GetResultAsync(WerkingsgebiedenWerdenGewijzigdScenario scenario)
    {
        var getResponse =
            await Context
                 .PublicElasticClient
                 .GetAsync<VerenigingZoekDocument>(scenario.WerkingsgebiedenWerdenGewijzigd.VCode);

        return getResponse.Source;
    }
}
