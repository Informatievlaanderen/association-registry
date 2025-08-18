namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Elastic.Clients.Elasticsearch;
using System;
using System.Threading.Tasks;

public class ProjectionRefresher
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly TimeSpan _postRefreshDelay;

    public ProjectionRefresher(
        ElasticsearchClient elasticClient,
        TimeSpan? postRefreshDelay = null)
    {
        _elasticClient = elasticClient;
        _postRefreshDelay = postRefreshDelay ?? TimeSpan.FromMilliseconds(500);
    }

    public async Task RefreshAsync()
    {
        await ForceRefreshIndices();
        await WaitForProcessing();
    }

    private async Task ForceRefreshIndices()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.All);
    }

    private async Task WaitForProcessing()
    {
        await Task.Delay(_postRefreshDelay);
    }
}
