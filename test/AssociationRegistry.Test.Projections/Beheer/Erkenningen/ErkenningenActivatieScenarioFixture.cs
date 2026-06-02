namespace AssociationRegistry.Test.Projections.Beheer.Erkenningen;

using Admin.ProjectionHost.Projections.Erkenningen;
using Admin.Schema.Erkenningen;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;

public class ErkenningenActivatieScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, ErkenningDocument[], ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon) =>
        await daemon.RebuildProjectionAsync<ErkenningProjection>(CancellationToken.None);

    protected override async Task<ErkenningDocument[]> GetResultAsync(
        IDocumentSession session,
        TScenario scenario)
        => (await session
                 .Query<ErkenningDocument>()
                 .Where(x => x.VCode == scenario.AggregateId)
                 .ToListAsync())
           .ToArray();
}

public class ErkenningenActivatieScenarioClassFixture<TScenario>
    : IClassFixture<ErkenningenActivatieScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
