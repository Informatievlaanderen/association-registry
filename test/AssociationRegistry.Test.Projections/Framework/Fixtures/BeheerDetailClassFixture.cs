namespace AssociationRegistry.Test.Projections.Framework.Fixtures;

using Admin.Schema.Detail;
using Marten;
using Xunit;

public class BeheerDetailClassFixture2<TScenario> : IClassFixture<BeheerDetailClassFixture<TScenario>>
    where TScenario : IScenario, new();

public class BeheerDetailClassFixture<TScenario> : ProjectionScenarioFixture<ProjectionContext, TScenario, BeheerVerenigingDetailDocument>
    where TScenario : IScenario, new()
{
    public BeheerDetailClassFixture(ProjectionContext context) : base(context, new TScenario())
    {
    }

    protected override async Task<BeheerVerenigingDetailDocument> FetchData()
    {
        return
            await Context
                 .Session
                 .Query<BeheerVerenigingDetailDocument>()
                 .Where(w => w.VCode == Scenario.VCode)
                 .SingleAsync();
    }
}
