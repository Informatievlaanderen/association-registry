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
        await EnsureElasticSearchIsInitialized(source.Services.GetRequiredService<ElasticsearchClient>(), source.Services.GetRequiredService<ElasticSearchOptionsSection>(),
                                               source.Services.GetRequiredService<ILogger<Program>>());
    }

    public static async Task EnsureElasticSearchIsInitialized(ElasticsearchClient elasticClient, ElasticSearchOptionsSection elasticSearchOptions, ILogger logger)
    {
        await WaitFor.ElasticSearchToBecomeAvailable(
            elasticClient, logger, cancellationToken: CancellationToken.None);

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
