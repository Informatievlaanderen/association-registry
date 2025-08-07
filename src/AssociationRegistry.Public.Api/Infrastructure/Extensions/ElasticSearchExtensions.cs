namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Transport;
using Hosts.Configuration.ConfigurationBindings;
using Schema.Search;
using System.Text;

public static class ElasticSearchExtensions
{
    public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        ElasticSearchOptionsSection elasticSearchOptions)
    {
        var elasticClient = (IServiceProvider serviceProvider)
            => CreateElasticClient(elasticSearchOptions, serviceProvider.GetRequiredService<ILogger<ElasticsearchClient>>());

        services.AddMappingsForVerenigingZoek(elasticSearchOptions.Indices!.Verenigingen!)
                .AddSingleton(sp => elasticClient(sp));

        return services;
    }

    private static IServiceCollection AddMappingsForVerenigingZoek(this IServiceCollection services, string indexName)
    {
        services.AddSingleton(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<ElasticsearchClient>();
            var mappingResponse = client.Indices.GetMapping(new GetMappingRequest(indexName));

            if (mappingResponse.IsValidResponse && mappingResponse.Mappings.TryGetValue(indexName, out var indexMapping))
            {
                return indexMapping.Mappings;
            }

            throw new InvalidOperationException($"Could not retrieve mapping for index '{indexName}'");
        });

        return services;
    }

    public static ElasticsearchClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions, ILogger logger)
    {
        var settings = new ElasticsearchClientSettings(new Uri(elasticSearchOptions.Uri!))
                      .Authentication(new BasicAuthentication(
                                          elasticSearchOptions.Username,
                                          elasticSearchOptions.Password))
                      .ServerCertificateValidationCallback((_, _, _, _) => true)
                      .MapVerenigingDocument(elasticSearchOptions.Indices!.Verenigingen!);

        if (elasticSearchOptions.EnableDevelopmentLogs)
        {
            settings = settings.DisableDirectStreaming()
                              .PrettyJson()
                              .OnRequestCompleted(apiCallDetails =>
                               {
                                   if (apiCallDetails.RequestBodyInBytes != null)
                                       logger.LogDebug(
                                           "{HttpMethod} {Uri} \n {RequestBody}",
                                           apiCallDetails.HttpMethod,
                                           apiCallDetails.Uri,
                                           Encoding.UTF8.GetString(apiCallDetails.RequestBodyInBytes));

                                   if (apiCallDetails.ResponseBodyInBytes != null)
                                       logger.LogDebug("Response: {ResponseBody}",
                                                       Encoding.UTF8.GetString(apiCallDetails.ResponseBodyInBytes));
                               });
        }

        return new ElasticsearchClient(settings);
    }

    public static ElasticsearchClientSettings MapVerenigingDocument(this ElasticsearchClientSettings settings, string indexName)
    {
        return settings.DefaultMappingFor(
            typeof(VerenigingZoekDocument),
            selector: descriptor => descriptor.IndexName(indexName)
                                              .IdProperty(nameof(VerenigingZoekDocument.VCode)));
    }
}
