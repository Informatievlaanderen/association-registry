namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Framework;
using Framework.Fixtures;
using Marten;
using Public.Schema.Detail;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenNietBepaaldFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenNietBepaaldScenario,
    PubliekVerenigingDetailDocument,
    ProjectionContext>(context)
{
    public override async Task<PubliekVerenigingDetailDocument> GetResultAsync(WerkingsgebiedenWerdenNietBepaaldScenario scenario)
        => await Context.Session
                        .Query<PubliekVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.WerkingsgebiedenWerdenNietBepaald.VCode);
}
