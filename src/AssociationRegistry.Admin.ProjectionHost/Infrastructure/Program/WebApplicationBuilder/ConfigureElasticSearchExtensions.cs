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
        var elasticClient = (IServiceProvider serviceProvider)
            => ElasticSearchExtensions.CreateElasticClient(elasticSearchOptions, serviceProvider.GetRequiredService<ILogger<ElasticClient>>());

        services.AddSingleton(elasticSearchOptions);

        services.AddSingleton(sp => elasticClient(sp));

        services.AddSingleton<IElasticClient>(sp => sp.GetRequiredService<ElasticClient>());

        return services;
    }
}
