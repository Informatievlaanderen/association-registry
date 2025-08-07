namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplication;

using Extensions;
using Hosts;
using Hosts.Configuration.ConfigurationBindings;
using Elastic.Clients.Elasticsearch;
using Program = ProjectionHost.Program;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

public static class PrepareElasticSearch
{
    public static async Task EnsureElasticSearchIsInitialized(this WebApplication source)
    {
        var elasticClient = source.Services.GetRequiredService<ElasticsearchClient>();
        var elasticSearchOptions = source.Services.GetRequiredService<ElasticSearchOptionsSection>();

        await WaitFor.ElasticSearchToBecomeAvailable(
            elasticClient, source.Services.GetRequiredService<ILogger<Program>>(), cancellationToken: CancellationToken.None);

        await EnsureIndexExists(elasticClient, elasticSearchOptions.Indices!.Verenigingen!);
    }

    private static async Task EnsureIndexExists(ElasticsearchClient elasticClient, string verenigingenIndexName)
    {
        if (!(await elasticClient.Indices.ExistsAsync(verenigingenIndexName)).Exists)
        {
            var response = await elasticClient.CreateVerenigingIndexAsync(verenigingenIndexName);

            if (!response.IsValidResponse)
                throw new InvalidOperationException($"Failed to create index: {response.DebugInformation}");
        }
    }
}
