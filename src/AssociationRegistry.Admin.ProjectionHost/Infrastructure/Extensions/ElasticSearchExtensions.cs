namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;

using ElasticSearch;
using Hosts.Configuration.ConfigurationBindings;
using Nest;
using Schema.Search;
using System.Text;

public static class ElasticSearchExtensions
{
    public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        ElasticSearchOptionsSection elasticSearchOptions, ILogger<IElasticClient> logger)
    {
        var elasticClient = CreateElasticClient(elasticSearchOptions, logger);

        EnsureIndexExists(elasticClient,
                          elasticSearchOptions.Indices!.Verenigingen!,
                          elasticSearchOptions.Indices!.DuplicateDetection!);

        services.AddSingleton(_ => elasticClient);
        services.AddSingleton<IElasticClient>(_ => elasticClient);

        return services;
    }

    public static void EnsureIndexExists(
        IElasticClient elasticClient,
        string verenigingenIndexName,
        string duplicateDetectionIndexName)
    {
        if (!elasticClient.Indices.Exists(verenigingenIndexName).Exists)
            elasticClient.Indices.CreateVerenigingIndex(verenigingenIndexName);

        if (!elasticClient.Indices.Exists(duplicateDetectionIndexName).Exists)
            elasticClient.Indices.CreateDuplicateDetectionIndex(duplicateDetectionIndexName);
    }

    public static ElasticClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions, ILogger logger)
    {
        var settings = new ConnectionSettings(new Uri(elasticSearchOptions.Uri!))
                      .BasicAuthentication(
                           elasticSearchOptions.Username,
                           elasticSearchOptions.Password)
                      .ServerCertificateValidationCallback((_, _, _, _) => true)
                      .MapVerenigingDocument(elasticSearchOptions.Indices!.Verenigingen!)
                      .MapDuplicateDetectionDocument(elasticSearchOptions.Indices!.DuplicateDetection!);

        if (elasticSearchOptions.EnableDevelopmentLogs)
            settings = settings.DisableDirectStreaming()
                               .PrettyJson()
                               .OnRequestCompleted(apiCallDetails =>
                                {
                                    if (apiCallDetails.RequestBodyInBytes != null)
                                        logger.LogDebug(
                                            message: "{HttpMethod} {Uri} \n {RequestBody}",
                                            apiCallDetails.HttpMethod,
                                            apiCallDetails.Uri,
                                            Encoding.UTF8.GetString(apiCallDetails.RequestBodyInBytes));

                                    if (apiCallDetails.ResponseBodyInBytes != null)
                                        logger.LogDebug(message: "Response: {ResponseBody}",
                                                        Encoding.UTF8.GetString(apiCallDetails.ResponseBodyInBytes));
                                });

        var elasticClient = new ElasticClient(settings);

        return elasticClient;
    }

    public static ConnectionSettings MapVerenigingDocument(this ConnectionSettings settings, string indexName)
    {
        return settings.DefaultMappingFor(
            typeof(VerenigingZoekDocument),
            selector: descriptor => descriptor.IndexName(indexName)
                                              .IdProperty(nameof(VerenigingZoekDocument.VCode)));
    }

    public static ConnectionSettings MapDuplicateDetectionDocument(this ConnectionSettings settings, string indexName)
    {
        return settings.DefaultMappingFor(
                            typeof(DuplicateDetectionDocument),
                            selector: descriptor => descriptor.IndexName(indexName)
                                                              .IdProperty(nameof(DuplicateDetectionDocument.VCode)))
                       .DefaultMappingFor(typeof(DuplicateDetectionUpdateDocument),
                                          selector: descriptor => descriptor.IndexName(indexName)
                                                                            .IdProperty(nameof(DuplicateDetectionDocument.VCode)));
    }
}
