namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Extensions;
using Hosts.Configuration.ConfigurationBindings;
using Nest;

public static class ConfigureElasticSearchExtensions
{
    public static IServiceCollection ConfigureElasticSearch(
        this IServiceCollection services,
        ElasticSearchOptionsSection elasticSearchOptions)
    {
        var elasticClient = CreateElasticClient(elasticSearchOptions);

        ElasticSearchExtensions.EnsureIndexExists(elasticClient,
                                                  elasticSearchOptions.Indices!.Verenigingen!,
                                                  elasticSearchOptions.Indices!.DuplicateDetection!);

        services.AddSingleton(elasticSearchOptions);

        services.AddSingleton(_ => elasticClient);
        services.AddSingleton<IElasticClient>(_ => elasticClient);

        return services;
    }

    private static ElasticClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions)
    {
        var settings = new ConnectionSettings(new Uri(elasticSearchOptions.Uri!))
                      .BasicAuthentication(
                           elasticSearchOptions.Username,
                           elasticSearchOptions.Password)
                      .MapVerenigingDocument(elasticSearchOptions.Indices!.Verenigingen!)
                      .MapDuplicateDetectionDocument(elasticSearchOptions.Indices.DuplicateDetection!);

        var elasticClient = new ElasticClient(settings);

        return elasticClient;
    }
}
