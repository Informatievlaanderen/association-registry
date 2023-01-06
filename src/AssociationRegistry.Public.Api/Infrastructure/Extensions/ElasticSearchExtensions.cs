﻿namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using System;
using ConfigurationBindings;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Schema.Search;

public static class ElasticSearchExtensions
{
   public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        ElasticSearchOptionsSection elasticSearchOptions)
    {
        var elasticClient = CreateElasticClient(elasticSearchOptions);

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
            .DefaultMappingFor(
                typeof(VerenigingDocument),
                descriptor => descriptor.IndexName(elasticSearchOptions.Indices!.Verenigingen));

        var elasticClient = new ElasticClient(settings);
        return elasticClient;
    }
}
