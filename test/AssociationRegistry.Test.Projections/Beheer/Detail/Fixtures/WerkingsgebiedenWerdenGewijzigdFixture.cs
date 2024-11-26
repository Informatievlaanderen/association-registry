namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.Schema.Detail;
using Framework;
using Framework.Fixtures;
using Marten;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenGewijzigdFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenGewijzigdScenario,
    BeheerVerenigingDetailDocument,
    ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingDetailDocument> GetResultAsync(WerkingsgebiedenWerdenGewijzigdScenario scenario)
        => await Context.Session
                        .Query<BeheerVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.WerkingsgebiedenWerdenGewijzigd.VCode);
}
