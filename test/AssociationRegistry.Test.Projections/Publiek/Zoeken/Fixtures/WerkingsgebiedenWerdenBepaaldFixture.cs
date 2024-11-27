namespace AssociationRegistry.Test.Projections.Publiek.Zoeken;

using Framework.Fixtures;
using Public.Schema.Search;
using Scenario;

public class WerkingsgebiedenWerdenBepaaldFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenBepaaldScenario,
    VerenigingZoekDocument,
    ProjectionContext>(context)
{
    protected override async Task<VerenigingZoekDocument> GetResultAsync(WerkingsgebiedenWerdenBepaaldScenario scenario)
    {
        var getResponse =
            await Context
                 .PublicElasticClient
                 .GetAsync<VerenigingZoekDocument>(scenario.WerkingsgebiedenWerdenBepaald.VCode);

        return getResponse.Source;
    }
}
