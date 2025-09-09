namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Transport;
using ElasticSearch;
using Hosts.Configuration.ConfigurationBindings;
using Schema;
using Schema.Setup.Elasticsearch;
using System.Text;

public static class ElasticSearchExtensions
{
    public static async Task EnsureIndicesExistsAsync(
        ElasticsearchClient elasticClient,
        string verenigingenIndexName,
        string duplicateDetectionIndexName)
    {
        await EnsureBeheerZoekVerenigingIndexExists(elasticClient, verenigingenIndexName);
        await EnsureDuplicateDetectionIndexExists(elasticClient, duplicateDetectionIndexName);
    }

    private static async Task EnsureDuplicateDetectionIndexExists(ElasticsearchClient elasticClient, string duplicateDetectionIndexName)
    {
        await EnsureIndexExistsAsync(elasticClient,
                                     duplicateDetectionIndexName,
                                     () => elasticClient.CreateDuplicateDetectionIndexAsync(duplicateDetectionIndexName));
    }

    private static async Task EnsureBeheerZoekVerenigingIndexExists(
        ElasticsearchClient elasticClient,
        string verenigingenIndexName)
    {
        await EnsureIndexExistsAsync(elasticClient, verenigingenIndexName,
                                     () => elasticClient.CreateVerenigingIndexAsync(verenigingenIndexName));
    }

    private static async Task EnsureIndexExistsAsync(
        ElasticsearchClient elasticClient,
        string indexName,
        Func<Task<CreateIndexResponse>> indexCreationFunc)
    {
        var indexExists = await elasticClient.Indices.ExistsAsync(indexName);

        if (!indexExists.Exists)
        {
            await indexCreationFunc();
        }
    }

    public static ElasticsearchClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions, ILogger logger)
    {
        var settings = new ElasticsearchClientSettings(new Uri(elasticSearchOptions.Uri!))
                      .Authentication(new BasicAuthentication(elasticSearchOptions.Username!, elasticSearchOptions.Password!))
                      .ServerCertificateValidationCallback((_, _, _, _) => true)
                      .MapAllVerenigingDocuments(elasticSearchOptions.Indices!.Verenigingen!,
                                                 elasticSearchOptions.Indices.DuplicateDetection!);

        if (elasticSearchOptions.EnableDevelopmentLogs)
        {
            settings = settings
                      .EnableDebugMode()
                      .PrettyJson()
                      .OnRequestCompleted(apiCall =>
                       {
                           if (apiCall.RequestBodyInBytes is { } requestBody)
                           {
                               logger.LogDebug("{Method} {Uri}\n{RequestBody}",
                                               apiCall.HttpMethod,
                                               apiCall.Uri,
                                               Encoding.UTF8.GetString(requestBody));
                           }

                           if (apiCall.ResponseBodyInBytes is { } responseBody)
                           {
                               logger.LogDebug("Response: {ResponseBody}", Encoding.UTF8.GetString(responseBody));
                           }
                       });
        }

        return new ElasticsearchClient(settings);
    }
}
