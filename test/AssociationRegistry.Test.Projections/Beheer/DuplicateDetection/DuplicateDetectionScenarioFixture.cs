namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection;

using Admin.ProjectionHost.Projections.Search.DuplicateDetection;
using Admin.Schema.Search;
using AssociationRegistry.Admin.ProjectionHost.Projections;
using Elastic.Clients.Elasticsearch;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;

public class DuplicateDetectionScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, DuplicateDetectionDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
    {
        await daemon.RebuildProjectionAsync(DuplicateDetectionProjectionHandler.ShardName.Name, CancellationToken.None);
        await Context.AdminElasticClient.Indices.RefreshAsync(Indices.All);
    }

    protected override async Task<DuplicateDetectionDocument> GetResultAsync(IDocumentSession _, TScenario scenario)
    {
        var getResponse = await Context.AdminElasticClient.GetAsync<DuplicateDetectionDocument>(scenario.AggregateId);

        return getResponse.Source;
    }
}

public class DuplicateDetectionClassFixture<TScenario> : IClassFixture<DuplicateDetectionScenarioFixture<TScenario>>
    where TScenario : IScenario, new() { }
