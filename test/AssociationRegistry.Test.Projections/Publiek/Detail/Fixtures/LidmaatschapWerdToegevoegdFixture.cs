namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Framework.Fixtures;
using Marten;
using Public.Schema.Detail;
using ScenarioClassFixtures;

public class LidmaatschapWerdToegevoegdFixture(ProjectionContext context)
    : ScenarioFixture<LidmaatschapWerdToegevoegdScenario, PubliekVerenigingDetailDocument, ProjectionContext>(context)
{
    public override async Task<PubliekVerenigingDetailDocument> GetResultAsync(LidmaatschapWerdToegevoegdScenario scenario)
        => await Context.Session
                        .Query<PubliekVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.LidmaatschapWerdToegevoegd.VCode);
}
