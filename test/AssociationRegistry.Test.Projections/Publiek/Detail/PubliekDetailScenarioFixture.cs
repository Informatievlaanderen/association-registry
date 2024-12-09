namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Framework.Fixtures;
using Marten;
using Marten.Events.Daemon;
using Public.ProjectionHost.Projections;
using Public.Schema.Detail;

public class PubliekDetailScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, PubliekVerenigingDetailDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.PublicStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
    {
        await daemon.RebuildProjectionAsync(ProjectionNames.PubliekDetail, CancellationToken.None);
    }

    protected override async Task<PubliekVerenigingDetailDocument> GetResultAsync(
        IDocumentSession session,
        TScenario scenario)
        => await session
                .Query<PubliekVerenigingDetailDocument>()
                .SingleAsync(x => x.VCode == scenario.VCode);
}

public class PubliekDetailScenarioClassFixture<TScenario>
    : IClassFixture<PubliekDetailScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
