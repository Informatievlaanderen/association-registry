namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema.Search;
using Framework;
using Framework.Fixtures;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenNietBepaaldFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenNietBepaaldScenario,
    VerenigingZoekDocument,
    ProjectionContext>(context)
{
    public override async Task<VerenigingZoekDocument> GetResultAsync(WerkingsgebiedenWerdenNietBepaaldScenario scenario)
    {
        var getResponse =
            await Context
                 .AdminElasticClient
                 .GetAsync<VerenigingZoekDocument>(scenario.WerkingsgebiedenWerdenNietBepaald.VCode);

        return getResponse.Source;
    }
}
