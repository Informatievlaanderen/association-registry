namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.ProjectionHost.Projections.Historiek;
using Admin.Schema.Historiek;
using Framework.Fixtures;
using Marten;
using Marten.Events.Daemon;

public class BeheerHistoriekScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, BeheerVerenigingHistoriekDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override async Task<BeheerVerenigingHistoriekDocument> GetResultAsync(
        TScenario scenario)
    {
        var store = Context.AdminStore;
        await using var session = store.LightweightSession();
        using var daemon = await store.BuildProjectionDaemonAsync();

        await daemon.RebuildProjectionAsync<BeheerVerenigingHistoriekProjection>(CancellationToken.None);
        return await session
                            .Query<BeheerVerenigingHistoriekDocument>()
                            .SingleAsync(x => x.VCode == scenario.VCode);
    }
}

public class BeheerHistoriekScenarioClassFixture<TScenario>
    : IClassFixture<BeheerHistoriekScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
