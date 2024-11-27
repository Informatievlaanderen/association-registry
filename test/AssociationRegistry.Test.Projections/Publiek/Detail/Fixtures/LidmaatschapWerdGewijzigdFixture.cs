namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Framework.Fixtures;
using Marten;
using Public.Schema.Detail;
using ScenarioClassFixtures;

public class LidmaatschapWerdGewijzigdFixture(ProjectionContext context)
    : ScenarioFixture<LidmaatschapWerdGewijzigdScenario, PubliekVerenigingDetailDocument, ProjectionContext>(context)
{
    public override async Task<PubliekVerenigingDetailDocument> GetResultAsync(LidmaatschapWerdGewijzigdScenario scenario)
        => await Context.Session
                        .Query<PubliekVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.LidmaatschapWerdGewijzigd.VCode);
}
