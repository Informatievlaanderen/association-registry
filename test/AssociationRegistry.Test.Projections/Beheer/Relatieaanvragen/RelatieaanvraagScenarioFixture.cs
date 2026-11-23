namespace AssociationRegistry.Test.Projections.Beheer.Relatieaanvragen;

using Admin.ProjectionHost.Projections.Relatieaanvragen;
using Admin.Schema.Relatieaanvragen;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;

public class RelatieaanvraagScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, RelatieaanvraagDocument?, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon) =>
        await daemon.RebuildProjectionAsync<RelatieaanvraagProjection>(CancellationToken.None);

    protected override async Task<RelatieaanvraagDocument?> GetResultAsync(
        IDocumentSession session,
        TScenario scenario
    ) => await session.LoadAsync<RelatieaanvraagDocument>(scenario.AggregateId);
}

public class RelatieaanvraagScenarioClassFixture<TScenario> : IClassFixture<RelatieaanvraagScenarioFixture<TScenario>>
    where TScenario : IScenario, new() { }
