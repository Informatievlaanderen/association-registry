namespace AssociationRegistry.Test.Projections.Bewaartermijn;

using Admin.ProjectionHost.Projections.Bewaartermijn;
using Admin.Schema.Bewaartermijn;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;

public class BewaartermijnScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, BewaartermijnDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
        => await daemon.RebuildProjectionAsync<BewaartermijnProjection>(CancellationToken.None);

    protected override async Task<BewaartermijnDocument> GetResultAsync(
        IDocumentSession session,
        TScenario scenario)
        => await session
                .Query<BewaartermijnDocument>()
                .SingleAsync(x => x.BewaartermijnId == scenario.AggregateId);
}

public class BewaartermijnScenarioClassFixture<TScenario>
    : IClassFixture<BewaartermijnScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
