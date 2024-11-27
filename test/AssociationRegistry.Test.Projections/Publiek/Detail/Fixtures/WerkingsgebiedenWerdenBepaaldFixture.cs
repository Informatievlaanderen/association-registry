namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Framework.Fixtures;
using Marten;
using Public.Schema.Detail;
using Scenario;

public class WerkingsgebiedenWerdenBepaaldFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenBepaaldScenario,
    PubliekVerenigingDetailDocument,
    ProjectionContext>(context)
{
    protected override async Task<PubliekVerenigingDetailDocument> GetResultAsync(WerkingsgebiedenWerdenBepaaldScenario scenario)
        => await Context.Session
                        .Query<PubliekVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.WerkingsgebiedenWerdenBepaald.VCode);
}
