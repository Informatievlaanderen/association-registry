namespace AssociationRegistry.Test.Projections.Framework;

using Alba;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Elastic.Clients.Elasticsearch;

public record ProjectionHostContext
{
    public ProjectionHostContext(IAlbaHost host)
    {
        Host = host;
        ElasticClient = Host.Services.GetRequiredService<ElasticsearchClient>();
    }

    public IAlbaHost Host { get; }
    public ElasticsearchClient ElasticClient { get; }

    public async Task RefreshDataAsync()
    {
        await Host.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await ElasticClient.Indices.RefreshAsync();
    }
}
