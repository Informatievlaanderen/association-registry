namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Hosts.Configuration.ConfigurationBindings;
using Nest;
using Schema;

public static class ConfigureElasticSearchExtensions
{
    public static IServiceCollection ConfigureElasticSearch(
        this IServiceCollection services,
        ElasticSearchOptionsSection elasticSearchOptions)
    {
        var elasticClient = CreateElasticClient(elasticSearchOptions);

        services.AddSingleton(elasticSearchOptions);

        services.AddSingleton(elasticClient);
        services.AddSingleton<IElasticClient>(provider => provider.GetRequiredService<ElasticClient>());

        return services;
    }

    public static ElasticClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions)
    {
        var settings = new ConnectionSettings(new Uri(elasticSearchOptions.Uri!))
                      .BasicAuthentication(
                           elasticSearchOptions.Username,
                           elasticSearchOptions.Password)
                      .ServerCertificateValidationCallback((_, _, _, _) => true)
                      .MapVerenigingDocument(elasticSearchOptions.Indices!.Verenigingen!);

        var elasticClient = new ElasticClient(settings);

        return elasticClient;
    }
}
