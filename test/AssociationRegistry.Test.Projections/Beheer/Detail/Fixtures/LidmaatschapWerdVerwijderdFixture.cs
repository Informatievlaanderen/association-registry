namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.Schema.Detail;
using Framework;
using Framework.Fixtures;
using Marten;
using ScenarioClassFixtures;

public class LidmaatschapWerdVerwijderdFixture(ProjectionContext context)
    : ScenarioFixture<LidmaatschapWerdVerwijderdScenario, BeheerVerenigingDetailDocument, ProjectionContext>(context)
{
    public override async Task<BeheerVerenigingDetailDocument> GetResultAsync(LidmaatschapWerdVerwijderdScenario scenario)
        => await Context.Session
                        .Query<BeheerVerenigingDetailDocument>()
                        .SingleAsync(w => w.VCode == scenario.LidmaatschapWerdVerwijderd.VCode);
}
