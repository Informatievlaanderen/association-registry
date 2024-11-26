namespace AssociationRegistry.Test.Projections.Framework.Fixtures;

using Admin.Schema.Detail;
using Admin.Schema.PowerBiExport;
using Marten;

public class PowerBiClassFixture<TScenario> : ProjectionScenarioFixture<ProjectionContext, TScenario, PowerBiExportDocument>
    where TScenario : IScenario, new()
{
    public PowerBiClassFixture(ProjectionContext context) : base(context, new TScenario())
    {
    }

    protected override async Task<PowerBiExportDocument> FetchData()
    {
        return
            await Context
                 .Session
                 .Query<PowerBiExportDocument>()
                 .Where(w => w.VCode == Scenario.VCode)
                 .SingleAsync();
    }
}
