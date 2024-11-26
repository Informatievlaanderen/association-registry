namespace AssociationRegistry.Test.Projections.Framework.Fixtures;

using Admin.Schema.Detail;
using Marten;

public class LocatieZonderAdresMatchClassFixture<TScenario> : ProjectionScenarioFixture<ProjectionContext, TScenario, LocatieZonderAdresMatchDocument>
    where TScenario : IScenario, new()
{
    public LocatieZonderAdresMatchClassFixture(ProjectionContext context) : base(context, new TScenario())
    {
    }

    protected override async Task<LocatieZonderAdresMatchDocument> FetchData()
    {
        return
            await Context
                 .Session
                 .Query<LocatieZonderAdresMatchDocument>()
                 .Where(w => w.VCode == Scenario.VCode)
                 .SingleAsync();
    }
}
