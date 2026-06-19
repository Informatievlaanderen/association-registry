namespace AssociationRegistry.Test.Public.Api.Fixtures;

using AssociationRegistry.MartenDb.Logging;
using AssociationRegistry.MartenDb.PubliekZoeken;
using AssociationRegistry.MartenDb.Subscriptions;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;
using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using JasperFx;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

internal static class PublicMartenSchemaHelper
{
    public static void ApplyPublicApiMartenSchemaChanges(IConfigurationRoot configuration)
    {
        var postgreSqlOptionsSection =
            AssociationRegistry.Public.Api.Infrastructure.Extensions.ConfigurationExtensions.GetPostgreSqlOptionsSection(
                configuration
            );

        using var documentStore = DocumentStore.For(options =>
        {
            AssociationRegistry.Public.Api.Infrastructure.Extensions.MartenExtensions.ConfigureStoreOptions(
                options,
                postgreSqlOptionsSection,
                NullLogger<SecureMartenLogger>.Instance,
                AutoCreate.All
            );
        });

        documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();
    }

    public static void ApplyPublicProjectionHostMartenSchemaChanges(IConfigurationRoot configuration)
    {
        var postgreSqlOptionsSection =
            AssociationRegistry.Hosts.Configuration.ConfigurationExtensions.GetPostgreSqlOptionsSection(configuration);
        var elasticSearchOptionsSection =
            AssociationRegistry.Hosts.Configuration.ConfigurationExtensions.GetElasticSearchOptionsSection(
                configuration
            );
        var elasticClient = ConfigureElasticSearchExtensions.CreateElasticClient(elasticSearchOptionsSection);

        using var documentStore = DocumentStore.For(options =>
        {
            ConfigureMartenExtensions.ConfigureStoreOptions(
                options,
                elasticClient,
                NullLogger<PubliekZoekenEventsConsumer>.Instance,
                NullLogger<MartenSubscription>.Instance,
                NullLogger<SecureMartenLogger>.Instance,
                postgreSqlOptionsSection,
                isDevelopment: true,
                elasticSearchOptionsSection: elasticSearchOptionsSection,
                autoCreate: AutoCreate.All
            );
        });

        documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();
    }
}
