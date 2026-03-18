namespace AssociationRegistry.Test.Projections.Beheer.LocatiesGekoppeldMetGrar;

using Admin.Schema.Locaties;
using AssociationRegistry.Admin.ProjectionHost.Projections.Locaties;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Test.Projections.Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;

public class LocatieLookupScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, LocatieLookupDocument[], ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon) =>
        await daemon.RebuildProjectionAsync<LocatiesGekoppeldMetGrarProjection>(CancellationToken.None);

    protected override async Task<LocatieLookupDocument[]> GetResultAsync(
        IDocumentSession session,
        TScenario scenario
    ) =>
        (
            await session.Query<LocatieLookupDocument>().Where(x => x.VCode == scenario.AggregateId).ToListAsync()
        ).ToArray();
}

public class LocatieLookupScenarioClassFixture<TScenario> : IClassFixture<LocatieLookupScenarioFixture<TScenario>>
    where TScenario : IScenario, new() { }
