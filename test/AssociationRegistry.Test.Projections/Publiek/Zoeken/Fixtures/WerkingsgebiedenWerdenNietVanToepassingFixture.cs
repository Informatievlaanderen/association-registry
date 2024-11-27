namespace AssociationRegistry.Test.Projections.Publiek.Zoeken;

using Framework.Fixtures;
using Public.Schema.Search;
using Scenario;

public class WerkingsgebiedenWerdenNietVanToepassingFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenNietVanToepassingScenario,
    VerenigingZoekDocument,
    ProjectionContext>(context)
{
    protected override async Task<VerenigingZoekDocument> GetResultAsync(WerkingsgebiedenWerdenNietVanToepassingScenario scenario)
    {
        var getResponse =
            await Context
                 .PublicElasticClient
                 .GetAsync<VerenigingZoekDocument>(scenario.WerkingsgebiedenWerdenNietVanToepassing.VCode);

        return getResponse.Source;
    }
}
