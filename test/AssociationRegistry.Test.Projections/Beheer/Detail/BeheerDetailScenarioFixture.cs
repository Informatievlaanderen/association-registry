namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.Schema.Detail;
using Framework.Fixtures;
using Marten;

public class BeheerDetailScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, BeheerVerenigingDetailDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override async Task<BeheerVerenigingDetailDocument> GetResultAsync(TScenario scenario)
        => await Context.Beheer.Session
                        .Query<BeheerVerenigingDetailDocument>()
                        .SingleAsync(x => x.VCode == scenario.VCode);
}

public class BeheerDetailScenarioClassFixture<TScenario>
    : IClassFixture<BeheerDetailScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
