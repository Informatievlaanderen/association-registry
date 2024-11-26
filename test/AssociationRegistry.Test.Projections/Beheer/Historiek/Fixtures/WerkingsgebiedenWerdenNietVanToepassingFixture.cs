namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Framework;
using Framework.Fixtures;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenNietVanToepassingFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenNietVanToepassingScenario,
    BeheerVerenigingHistoriekDocument,
    ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingHistoriekDocument> GetResultAsync(WerkingsgebiedenWerdenNietVanToepassingScenario scenario)
    {
        var getResponse =
            await Context
                 .AdminElasticClient
                 .GetAsync<BeheerVerenigingHistoriekDocument>(scenario.WerkingsgebiedenWerdenNietVanToepassing.VCode);

        return getResponse.Source;
    }
}
