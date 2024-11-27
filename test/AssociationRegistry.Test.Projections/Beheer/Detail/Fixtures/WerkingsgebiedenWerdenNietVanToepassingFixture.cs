namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.Schema.Detail;
using Framework.Fixtures;
using Marten;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenNietVanToepassingFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenNietVanToepassingScenario,
    BeheerVerenigingDetailDocument,
    ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingDetailDocument> GetResultAsync(WerkingsgebiedenWerdenNietVanToepassingScenario scenario)
        => await Context.Session
                        .Query<BeheerVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.WerkingsgebiedenWerdenNietVanToepassing.VCode);
}
