namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema.Search;
using Framework;
using Framework.Fixtures;
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
                 .AdminElasticClient
                 .GetAsync<VerenigingZoekDocument>(scenario.WerkingsgebiedenWerdenGewijzigd.VCode);

        return getResponse.Source;
    }
}
