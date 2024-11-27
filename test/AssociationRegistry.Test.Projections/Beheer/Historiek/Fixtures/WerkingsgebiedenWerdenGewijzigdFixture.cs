namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Framework.Fixtures;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenGewijzigdFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenGewijzigdScenario,
    BeheerVerenigingHistoriekDocument,
    ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingHistoriekDocument> GetResultAsync(WerkingsgebiedenWerdenGewijzigdScenario scenario)
    {
        var getResponse =
            await Context
                 .AdminElasticClient
                 .GetAsync<BeheerVerenigingHistoriekDocument>(scenario.WerkingsgebiedenWerdenGewijzigd.VCode);

        return getResponse.Source;
    }
}
