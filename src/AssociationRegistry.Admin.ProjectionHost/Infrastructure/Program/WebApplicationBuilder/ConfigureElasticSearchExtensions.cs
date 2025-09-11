namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Hosts.Configuration.ConfigurationBindings;
using Schema;
using Schema.Setup.Elasticsearch;

public static class ConfigureElasticSearchExtensions
{
    public static IServiceCollection ConfigureElasticSearch(
        this IServiceCollection services,
        ElasticSearchOptionsSection elasticSearchOptions)
    {
        var elasticClient = CreateElasticClient(elasticSearchOptions);

        services.AddSingleton(elasticSearchOptions);
        services.AddSingleton(elasticClient);

        return services;
    }

    private static ElasticsearchClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions)
    {
        var settings = new ElasticsearchClientSettings(new Uri(elasticSearchOptions.Uri!))
                      .Authentication(new BasicAuthentication(
                                          elasticSearchOptions.Username!,
                                          elasticSearchOptions.Password!))
                      .ServerCertificateValidationCallback((_, _, _, _) => true)
                      .MapAllVerenigingDocuments(elasticSearchOptions.Indices!.Verenigingen!, elasticSearchOptions.Indices.DuplicateDetection!);

        var elasticClient = new ElasticsearchClient(settings);

        return elasticClient;
    }
}
