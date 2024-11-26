namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Framework;
using Framework.Fixtures;
using Marten;
using Public.Schema.Detail;
using ScenarioClassFixtures;

public class LidmaatschapWerdVerwijderdFixture(ProjectionContext context)
    : ScenarioFixture<LidmaatschapWerdVerwijderdScenario, PubliekVerenigingDetailDocument, ProjectionContext>(context)
{
    public override async Task<PubliekVerenigingDetailDocument> GetResultAsync(LidmaatschapWerdVerwijderdScenario scenario)
        => await Context.Session
                        .Query<PubliekVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.LidmaatschapWerdVerwijderd.VCode);
}
