namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Framework.Fixtures;
using Marten;
using Public.Schema.Detail;

public class PubliekDetailScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, PubliekVerenigingDetailDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override async Task<PubliekVerenigingDetailDocument> GetResultAsync(TScenario scenario)
        => await Context.Publiek.Session
                        .Query<PubliekVerenigingDetailDocument>()
                        .SingleAsync(x => x.VCode == scenario.VCode);
}

public class PubliekDetailScenarioClassFixture<TScenario>
    : IClassFixture<PubliekDetailScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
