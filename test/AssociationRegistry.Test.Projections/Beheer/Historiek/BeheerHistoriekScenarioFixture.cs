namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Framework.Fixtures;
using Marten;

public class BeheerHistoriekScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, BeheerVerenigingHistoriekDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override async Task<BeheerVerenigingHistoriekDocument> GetResultAsync(TScenario scenario)
        => await Context.Session
                        .Query<BeheerVerenigingHistoriekDocument>()
                        .SingleAsync(x => x.VCode == scenario.VCode);
}

public class BeheerHistoriekScenarioClassFixture<TScenario>
    : IClassFixture<BeheerHistoriekScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
