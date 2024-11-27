namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema.Search;
using Framework.Fixtures;
using Scenario;

public class WerkingsgebiedenWerdenNietBepaaldFixture(ProjectionContext context)
    : ScenarioFixture<WerkingsgebiedenWerdenNietBepaaldScenario, VerenigingZoekDocument, ProjectionContext>(context)
{
    protected override async Task<VerenigingZoekDocument> GetResultAsync(WerkingsgebiedenWerdenNietBepaaldScenario scenario)
    {
        var getResponse =
            await Context
                 .AdminElasticClient
                 .GetAsync<VerenigingZoekDocument>(scenario.WerkingsgebiedenWerdenNietBepaald.VCode);

        return getResponse.Source;
    }
}
