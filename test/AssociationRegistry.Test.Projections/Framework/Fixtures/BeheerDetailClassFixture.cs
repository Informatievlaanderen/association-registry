namespace AssociationRegistry.Test.Projections.Framework.Fixtures;

using Admin.Schema.Detail;
using Marten;

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
