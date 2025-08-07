namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Hosts.Configuration.ConfigurationBindings;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Schema;
using Schema.Search;

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

    public static ElasticsearchClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions)
    {
        var settings = new ElasticsearchClientSettings(new Uri(elasticSearchOptions.Uri!))
                      .Authentication(new BasicAuthentication(
                                          elasticSearchOptions.Username!,
                                          elasticSearchOptions.Password!))
                      .ServerCertificateValidationCallback((_, _, _, _) => true)
                      .MapVerenigingDocument(elasticSearchOptions.Indices!.Verenigingen!);

        var elasticClient = new ElasticsearchClient(settings);

        return elasticClient;
    }
}
