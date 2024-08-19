namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;

using ConfigurationBindings;
using Hosts.Configuration.ConfigurationBindings;
using Nest;
using Schema.Search;

public static class ElasticSearchExtensions
{
    public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        ElasticSearchOptionsSection elasticSearchOptions,
        ILogger logger)
    {
        var elasticClient = CreateElasticClient(elasticSearchOptions);

        EnsureIndexExists(elasticClient,
                          elasticSearchOptions.Indices!.Verenigingen!,
                          elasticSearchOptions.Indices!.DuplicateDetection!,
            logger);

        services.AddSingleton(_ => elasticClient);
        services.AddSingleton<IElasticClient>(_ => elasticClient);

        return services;
    }

    public static void EnsureIndexExists(
        IElasticClient elasticClient,
        string verenigingenIndexName,
        string duplicateDetectionIndexName,
        ILogger logger)
    {
        if (!elasticClient.Indices.Exists(verenigingenIndexName).Exists)
            elasticClient.Indices.CreateVerenigingIndex(verenigingenIndexName, logger);

        if (!elasticClient.Indices.Exists(duplicateDetectionIndexName).Exists)
            elasticClient.Indices.CreateDuplicateDetectionIndex(duplicateDetectionIndexName, logger);
    }

    private static ElasticClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions)
    {
        var settings = new ConnectionSettings(new Uri(elasticSearchOptions.Uri!))
                      .BasicAuthentication(
                           elasticSearchOptions.Username,
                           elasticSearchOptions.Password)
                      .ServerCertificateValidationCallback((_, _, _, _) => true)
                      .MapVerenigingDocument(elasticSearchOptions.Indices!.Verenigingen!)
                      .MapDuplicateDetectionDocument(elasticSearchOptions.Indices!.DuplicateDetection!);

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
                                              .IdProperty(nameof(DuplicateDetectionDocument.VCode)));
    }
}
