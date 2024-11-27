namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Framework.Fixtures;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenBepaaldFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenBepaaldScenario,
    BeheerVerenigingHistoriekDocument,
    ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingHistoriekDocument> GetResultAsync(WerkingsgebiedenWerdenBepaaldScenario scenario)
    {
        var getResponse =
            await Context
                 .AdminElasticClient
                 .GetAsync<BeheerVerenigingHistoriekDocument>(scenario.WerkingsgebiedenWerdenBepaald.VCode);

        return getResponse.Source;
    }
}
