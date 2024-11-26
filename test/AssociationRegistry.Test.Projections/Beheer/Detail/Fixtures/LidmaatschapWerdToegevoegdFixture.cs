namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.Schema.Detail;
using Framework;
using Framework.Fixtures;
using Marten;
using ScenarioClassFixtures;

public class LidmaatschapWerdToegevoegdFixture(ProjectionContext context)
    : ScenarioFixture<LidmaatschapWerdToegevoegdScenario, BeheerVerenigingDetailDocument, ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingDetailDocument> GetResultAsync(LidmaatschapWerdToegevoegdScenario scenario)
        => await Context.Session
                        .Query<BeheerVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.LidmaatschapWerdToegevoegd.VCode);
}
