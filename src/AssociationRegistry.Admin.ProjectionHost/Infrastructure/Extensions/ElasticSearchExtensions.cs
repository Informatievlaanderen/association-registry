namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;

using System;
using ConfigurationBindings;
using Schema.Search;
using Microsoft.Extensions.DependencyInjection;
using Nest;

public static class ElasticSearchExtensions
{
    public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        ElasticSearchOptionsSection elasticSearchOptions)
    {
        var elasticClient = CreateElasticClient(elasticSearchOptions);
        EnsureIndexExists(elasticClient, elasticSearchOptions.Indices!.Verenigingen!);

        services.AddSingleton(_ => elasticClient);
        services.AddSingleton<IElasticClient>(_ => elasticClient);

        return services;
    }

    private static void EnsureIndexExists(IElasticClient elasticClient, string verenigingenIndexName)
    {
        if (!elasticClient.Indices.Exists(verenigingenIndexName).Exists)
            elasticClient.Indices.CreateVerenigingIndex(verenigingenIndexName);
    }

    private static ElasticClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions)
    {
        var settings = new ConnectionSettings(new Uri(elasticSearchOptions.Uri!))
            .BasicAuthentication(
                elasticSearchOptions.Username,
                elasticSearchOptions.Password)
            .MapVerenigingDocument(elasticSearchOptions.Indices!.Verenigingen!);

        var elasticClient = new ElasticClient(settings);
        return elasticClient;
    }

    public static ConnectionSettings MapVerenigingDocument(this ConnectionSettings settings, string indexName)
    {
        return settings.DefaultMappingFor(
            typeof(VerenigingZoekDocument),
            descriptor => descriptor.IndexName(indexName)
                .IdProperty(nameof(VerenigingZoekDocument.VCode)));
    }
}
