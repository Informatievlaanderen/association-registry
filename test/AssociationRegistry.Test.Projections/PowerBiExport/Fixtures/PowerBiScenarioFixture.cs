namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Admin.ProjectionHost.Projections.PowerBiExport;
using Admin.Schema.PowerBiExport;
using Framework.Fixtures;
using Marten;
using Marten.Events.Daemon;

public class PowerBiScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, PowerBiExportDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override async Task<PowerBiExportDocument> GetResultAsync(
        TScenario scenario)
    {
        var store = Context.AdminStore;
        await using var session = store.LightweightSession();
        using var daemon = await store.BuildProjectionDaemonAsync();

        await daemon.RebuildProjectionAsync<PowerBiExportProjection>(CancellationToken.None);
        return await session
                    .Query<PowerBiExportDocument>()
                    .SingleAsync(x => x.VCode == scenario.VCode);
    }
}

public class PowerBiScenarioClassFixture<TScenario>
    : IClassFixture<PowerBiScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
