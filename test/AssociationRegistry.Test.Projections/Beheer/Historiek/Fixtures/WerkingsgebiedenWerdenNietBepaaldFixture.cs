namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Framework.Fixtures;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenNietBepaaldFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenNietBepaaldScenario,
    BeheerVerenigingHistoriekDocument,
    ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingHistoriekDocument> GetResultAsync(WerkingsgebiedenWerdenNietBepaaldScenario scenario)
    {
        var getResponse =
            await Context
                 .AdminElasticClient
                 .GetAsync<BeheerVerenigingHistoriekDocument>(scenario.WerkingsgebiedenWerdenNietBepaald.VCode);

        return getResponse.Source;
    }
}
