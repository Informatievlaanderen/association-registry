namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using Schema.Search;
using System;
using System.Text;

public static class ElasticSearchExtensions
{
    public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        ElasticSearchOptionsSection elasticSearchOptions)
    {
        var elasticClient = (IServiceProvider serviceProvider)
            => CreateElasticClient(elasticSearchOptions, serviceProvider.GetRequiredService<ILogger<ElasticClient>>());

        services.AddMappingsForVerenigingZoek(elasticSearchOptions.Indices!.Verenigingen!);

        services.AddSingleton(sp => elasticClient(sp));
        services.AddSingleton<IElasticClient>(serviceProvider => serviceProvider.GetRequiredService<ElasticClient>());

        return services;
    }

    private static IServiceCollection AddMappingsForVerenigingZoek(this IServiceCollection services, string indexName)
        => services.AddSingleton(
            serviceProvider => serviceProvider
                              .GetMappingFor<VerenigingZoekDocument>()
                              .Indices[indexName]
                              .Mappings);

    private static GetMappingResponse GetMappingFor<T>(this IServiceProvider serviceProvider) where T : class
        => serviceProvider.GetRequiredService<ElasticClient>().Indices.GetMapping<T>();

    private static ElasticClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions, ILogger logger)
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

        return new ElasticClient(settings);
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
