namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Framework.Fixtures;
using Marten;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenGewijzigdFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenGewijzigdScenario,
    BeheerVerenigingHistoriekDocument,
    ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingHistoriekDocument> GetResultAsync(WerkingsgebiedenWerdenGewijzigdScenario scenario)
        => await Context.Session
                        .Query<BeheerVerenigingHistoriekDocument>()
                        .SingleAsync(w => w.VCode == scenario.WerkingsgebiedenWerdenGewijzigd.VCode);
}
