namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.Schema.Detail;
using Framework.Fixtures;
using Marten;
using ScenarioClassFixtures;

public class WerkingsgebiedenWerdenNietBepaaldFixture(ProjectionContext context) : ScenarioFixture<
    WerkingsgebiedenWerdenNietBepaaldScenario,
    BeheerVerenigingDetailDocument,
    ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingDetailDocument> GetResultAsync(WerkingsgebiedenWerdenNietBepaaldScenario scenario)
        => await Context.Session
                        .Query<BeheerVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.WerkingsgebiedenWerdenNietBepaald.VCode);
}
