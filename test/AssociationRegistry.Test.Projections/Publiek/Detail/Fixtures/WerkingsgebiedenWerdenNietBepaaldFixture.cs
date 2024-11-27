namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Framework.Fixtures;
using Marten;
using Public.Schema.Detail;
using Scenario;

public class WerkingsgebiedenWerdenNietBepaaldFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenNietBepaaldScenario,
    PubliekVerenigingDetailDocument,
    ProjectionContext>(context)
{
    protected override async Task<PubliekVerenigingDetailDocument> GetResultAsync(WerkingsgebiedenWerdenNietBepaaldScenario scenario)
        => await Context.Session
                        .Query<PubliekVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.WerkingsgebiedenWerdenNietBepaald.VCode);
}
