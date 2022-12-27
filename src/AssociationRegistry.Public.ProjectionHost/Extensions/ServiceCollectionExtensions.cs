namespace AssociationRegistry.Public.ProjectionHost.Extensions;

using System;
using System.Linq;
using System.Reflection;
using ConfigurationBindings;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Projections.Search;

public static class ServiceCollectionExtensions
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
            .DefaultMappingFor(
                typeof(VerenigingDocument),
                descriptor => descriptor.IndexName(elasticSearchOptions.Indices!.Verenigingen));

        var elasticClient = new ElasticClient(settings);
        return elasticClient;
    }

    public static IServiceCollection RegisterDomainEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        assembly.GetTypes()
            .Where(
                item => item.GetInterfaces()
                            .Where(i => i.IsGenericType)
                            .Any(i => i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
                        && !item.IsAbstract && !item.IsInterface)
            .ToList()
            .ForEach(
                serviceType =>
                {
                    // allow only 1 eventhandler per class
                    var interfaceType = serviceType.GetInterfaces().Single(i => i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));
                    services.AddScoped(interfaceType, serviceType);
                });
        return services;
    }
}
