namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema.Search;
using Framework.Fixtures;
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
                 .AdminElasticClient
                 .GetAsync<VerenigingZoekDocument>(scenario.WerkingsgebiedenWerdenNietVanToepassing.VCode);

        return getResponse.Source;
    }
}
