namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Framework;
using Framework.Fixtures;
using Marten;
using Public.Schema.Detail;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenNietVanToepassingFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenNietVanToepassingScenario,
    PubliekVerenigingDetailDocument,
    ProjectionContext>(context)
{
    public override async Task<PubliekVerenigingDetailDocument> GetResultAsync(WerkingsgebiedenWerdenNietVanToepassingScenario scenario)
        => await Context.Session
                        .Query<PubliekVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.WerkingsgebiedenWerdenNietVanToepassing.VCode);
}
