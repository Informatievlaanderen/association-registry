namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;
using Marten.Events.Daemon;

public class BeheerDetailScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, BeheerVerenigingDetailDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
        => await daemon.RebuildProjectionAsync<BeheerVerenigingDetailProjection>(CancellationToken.None);

    protected override async Task<BeheerVerenigingDetailDocument> GetResultAsync(
        IDocumentSession session,
        TScenario scenario)
        => await session
                .Query<BeheerVerenigingDetailDocument>()
                .SingleAsync(x => x.VCode == scenario.VCode);
}

public class BeheerDetailScenarioClassFixture<TScenario>
    : IClassFixture<BeheerDetailScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
