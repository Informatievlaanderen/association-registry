﻿namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using ConfigurationBindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using Schema;
using Schema.Search;
using System;
using System.Text;

public static class ElasticSearchExtensions
{
    public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        ElasticSearchOptionsSection elasticSearchOptions)
    {
        services.AddSingleton<ElasticClient>(serviceProvider => CreateElasticClient(elasticSearchOptions, serviceProvider.GetRequiredService<ILogger<ElasticClient>>()));
        services.AddSingleton<IElasticClient>(serviceProvider => CreateElasticClient(elasticSearchOptions, serviceProvider.GetRequiredService<ILogger<ElasticClient>>()));

        services.AddSingleton(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IElasticClient>();
            var mapping = client.Indices.GetMapping<VerenigingZoekDocument>();

            return mapping.Indices[elasticSearchOptions.Indices!.Verenigingen!].Mappings;
        });

        return services;
    }

    private static ElasticClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions, ILogger logger)
    {
        var settings = new ConnectionSettings(new Uri(elasticSearchOptions.Uri!))
                      .BasicAuthentication(
                           elasticSearchOptions.Username,
                           elasticSearchOptions.Password)
                     .CertificateFingerprint(elasticSearchOptions.Fingerprint)
                     .ServerCertificateValidationCallback((o, certificate, arg3, arg4) =>
                      {
                          logger.LogWarning("Policy errors: [{Cert}] {Error}", certificate.Subject, arg4.ToString());
                          return true;
                      })
                      .IncludeServerStackTraceOnError()
                      .MapVerenigingDocument(elasticSearchOptions.Indices!.Verenigingen!);

        if (elasticSearchOptions.EnableDevelopmentLogs)
            settings = settings.DisableDirectStreaming()
                               .PrettyJson()
                               .OnRequestCompleted(apiCallDetails =>
                                {
                                    if (apiCallDetails.RequestBodyInBytes != null)
                                        logger.LogDebug(
                                            message: "ES Request: {HttpMethod} {Uri} \n {RequestBody}",
                                            apiCallDetails.HttpMethod,
                                            apiCallDetails.Uri,
                                            Encoding.UTF8.GetString(apiCallDetails.RequestBodyInBytes));

                                    if (apiCallDetails.ResponseBodyInBytes != null)
                                        logger.LogDebug(message: "ES Response: {ResponseBody}",
                                                        Encoding.UTF8.GetString(apiCallDetails.ResponseBodyInBytes));

                                    if (apiCallDetails.DebugInformation != null)
                                        logger.LogDebug(message: "ES Debug: {ResponseBody}",
                                                        apiCallDetails.DebugInformation);
                                });

        return new ElasticClient(settings);
    }
}
