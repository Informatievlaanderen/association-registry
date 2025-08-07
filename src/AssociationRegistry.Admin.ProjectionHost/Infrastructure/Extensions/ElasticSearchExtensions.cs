namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;

using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using ElasticSearch;
using Hosts.Configuration.ConfigurationBindings;
using Schema;
using System.Text;

public static class ElasticSearchExtensions
{
    public static async Task EnsureIndexExistsAsync(
        ElasticsearchClient elasticClient,
        string verenigingenIndexName,
        string duplicateDetectionIndexName)
    {
        var verenigingenExists = await elasticClient.Indices.ExistsAsync(verenigingenIndexName);

        if (!verenigingenExists.Exists)
        {
            await elasticClient.CreateVerenigingIndexAsync(verenigingenIndexName);
        }

        var duplicateExists = await elasticClient.Indices.ExistsAsync(duplicateDetectionIndexName);

        if (!duplicateExists.Exists)
        {
            await elasticClient.CreateDuplicateDetectionIndexAsync(duplicateDetectionIndexName);
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
