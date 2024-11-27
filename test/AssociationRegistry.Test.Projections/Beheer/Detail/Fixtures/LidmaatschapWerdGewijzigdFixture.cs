namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.Schema.Detail;
using Framework.Fixtures;
using Marten;
using ScenarioClassFixtures;

public class LidmaatschapWerdGewijzigdFixture(ProjectionContext context)
    : ScenarioFixture<LidmaatschapWerdGewijzigdScenario, BeheerVerenigingDetailDocument, ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingDetailDocument> GetResultAsync(LidmaatschapWerdGewijzigdScenario scenario)
        => await Context.Session
                        .Query<BeheerVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.LidmaatschapWerdGewijzigd.VCode);
}
