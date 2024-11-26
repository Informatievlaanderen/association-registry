namespace AssociationRegistry.Test.Projections.Framework.Fixtures;

using Admin.Schema.Historiek;
using Marten;

public class HistoriekClassFixture<TScenario> : ProjectionScenarioFixture<ProjectionContext, TScenario, BeheerVerenigingHistoriekDocument>
    where TScenario : IScenario, new()
{
    public HistoriekClassFixture(ProjectionContext context) : base(context, new TScenario())
    {
    }

    protected override async Task<BeheerVerenigingHistoriekDocument> FetchData()
    {
        return
            await Context
                 .Session
                 .Query<BeheerVerenigingHistoriekDocument>()
                 .Where(w => w.VCode == Scenario.VCode)
                 .SingleAsync();
    }
}
