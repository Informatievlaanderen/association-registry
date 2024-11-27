namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Framework.Fixtures;
using Marten;
using Public.Schema.Detail;
using Scenario;

public class LidmaatschapWerdGewijzigdFixture(ProjectionContext context)
    : ScenarioFixture<LidmaatschapWerdGewijzigdScenario, PubliekVerenigingDetailDocument, ProjectionContext>(context)
{
    protected override async Task<PubliekVerenigingDetailDocument> GetResultAsync(LidmaatschapWerdGewijzigdScenario scenario)
        => await Context.Session
                        .Query<PubliekVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.LidmaatschapWerdGewijzigd.VCode);
}
