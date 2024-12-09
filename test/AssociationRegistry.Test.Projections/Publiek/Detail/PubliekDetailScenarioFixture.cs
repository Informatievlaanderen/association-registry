namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Framework.Fixtures;
using Marten;
using Marten.Events.Daemon;
using Public.ProjectionHost.Projections.Detail;
using Public.Schema.Detail;

public class PubliekDetailScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, PubliekVerenigingDetailDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override async Task<PubliekVerenigingDetailDocument> GetResultAsync(
        TScenario scenario)
    {
        var store = Context.PublicStore;
        await using var session = store.LightweightSession();
        using var daemon = await store.BuildProjectionDaemonAsync();

        await daemon.RebuildProjectionAsync<PubliekVerenigingDetailProjection>(CancellationToken.None);
        return await session
                            .Query<PubliekVerenigingDetailDocument>()
                            .SingleAsync(x => x.VCode == scenario.VCode);
    }
}

public class PubliekDetailScenarioClassFixture<TScenario>
    : IClassFixture<PubliekDetailScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
