namespace AssociationRegistry.Public.Api.Extensions;

using System;
using System.Linq;
using System.Reflection;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Be.Vlaanderen.Basisregisters.BlobStore.Aws;
using Caches;
using ConfigurationBindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using Projections;
using S3;
using SearchVerenigingen;

public static class ServiceCollectionExtensions
{
    public static void AddBlobClients(this IServiceCollection services, S3BlobClientOptions s3Options) =>
        services
            .AddSingleton(sp => new VerenigingenBlobClient(
                s3Options.Buckets[WellknownBuckets.Verenigingen.Name],
                bucketName => new S3BlobClient(sp.GetRequiredService<AmazonS3Client>(), bucketName),
                sp.GetRequiredService<ILogger<VerenigingenBlobClient>>()
            ));

    public static void AddS3(this IServiceCollection services, IConfiguration configuration)
    {
        // Use MINIO
        if (configuration.GetValue<string>("MINIO_SERVER") != null)
        {
            if (configuration.GetValue<string>("MINIO_ROOT_USER") == null)
            {
                throw new InvalidOperationException("The MINIO_ROOT_USER configuration variable was not set.");
            }

            if (configuration.GetValue<string>("MINIO_ROOT_PASSWORD") == null)
            {
                throw new InvalidOperationException("The MINIO_ROOT_PASSWORD configuration variable was not set.");
            }

            services.AddSingleton(new AmazonS3Client(
                    new BasicAWSCredentials(
                        configuration.GetValue<string>("MINIO_ROOT_USER"),
                        configuration.GetValue<string>("MINIO_ROOT_PASSWORD")),
                    new AmazonS3Config
                    {
                        RegionEndpoint = RegionEndpoint.USEast1, // minio's default region
                        ServiceURL = configuration.GetValue<string>("MINIO_SERVER"),
                        ForcePathStyle = true,
                    }
                )
            );
        }
        else // Use AWS
        {
            services.AddSingleton(new AmazonS3Client());
        }
    }

    public static void AddDataCache(this IServiceCollection services) =>
        services.AddSingleton<IVerenigingenRepository>(sp =>
            VerenigingenRepository.Load(sp.GetRequiredService<VerenigingenBlobClient>()).GetAwaiter().GetResult());

    public static void AddJsonLdContexts(this IServiceCollection services)
    {
        services.AddSingleton<IVerenigingenRepository>(sp =>
            VerenigingenRepository.Load(sp.GetRequiredService<VerenigingenBlobClient>()).GetAwaiter().GetResult());
    }

    public static IServiceCollection AddElasticSearch(
        this IServiceCollection services,
        ElasticSearchOptionsSection elasticSearchOptions)
    {
        var elasticClient = CreateElasticClient(elasticSearchOptions);
        EnsureIndexExists(elasticClient, elasticSearchOptions.Indices.Verenigingen);

        services.AddSingleton(_ => elasticClient);
        services.AddSingleton<IElasticClient>(_ => elasticClient);

        return services;
    }

    private static void EnsureIndexExists(IElasticClient elasticClient, string verenigingenIndexName)
    {
        if (!elasticClient.Indices.Exists(verenigingenIndexName).Exists)
            elasticClient.Indices.Create(verenigingenIndexName);
    }

    private static ElasticClient CreateElasticClient(ElasticSearchOptionsSection elasticSearchOptions)
    {
        var settings = new ConnectionSettings(new Uri(elasticSearchOptions.Uri))
            .BasicAuthentication(
                elasticSearchOptions.Username,
                elasticSearchOptions.Password)
            .DefaultMappingFor(
                typeof(VerenigingDocument),
                descriptor => descriptor.IndexName(elasticSearchOptions.Indices.Verenigingen));

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
