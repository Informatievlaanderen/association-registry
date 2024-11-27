namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Framework.Fixtures;
using Marten;
using Public.Schema.Detail;
using Scenario;

public class WerkingsgebiedenWerdenGewijzigdFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenGewijzigdScenario,
    PubliekVerenigingDetailDocument,
    ProjectionContext>(context)
{
    protected override async Task<PubliekVerenigingDetailDocument> GetResultAsync(WerkingsgebiedenWerdenGewijzigdScenario scenario)
        => await Context.Session
                        .Query<PubliekVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.WerkingsgebiedenWerdenGewijzigd.VCode);
}
