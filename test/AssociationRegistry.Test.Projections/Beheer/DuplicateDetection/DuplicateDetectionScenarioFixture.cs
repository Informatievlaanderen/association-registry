namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection;

using Admin.Schema.Search;
using AssociationRegistry.Admin.ProjectionHost.Projections;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;
using Elastic.Clients.Elasticsearch;

public class DuplicateDetectionScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, DuplicateDetectionDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
    {
        await daemon.RebuildProjectionAsync(ProjectionNames.DuplicateDetection, CancellationToken.None);
        await Context.AdminElasticClient.Indices.RefreshAsync(Indices.All);
    }

    protected override async Task<DuplicateDetectionDocument> GetResultAsync(
        IDocumentSession _,
        TScenario scenario)
    {
        var getResponse =
            await Context.AdminElasticClient
                         .GetAsync<DuplicateDetectionDocument>(scenario.VCode);

        return getResponse.Source;
    }
}

public class DuplicateDetectionClassFixture<TScenario>
    : IClassFixture<DuplicateDetectionScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
