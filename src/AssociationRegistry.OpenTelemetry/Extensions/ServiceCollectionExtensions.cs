namespace AssociationRegistry.OpenTelemetry.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class ServiceCollectionExtensions
{
    public static void AddBlobClients(this IServiceCollection services, S3BlobClientOptions s3Options)
        => services
            .AddSingleton(
                sp => new VerenigingenBlobClient(
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

            services.AddSingleton(
                new AmazonS3Client(
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

    public static void AddDataCache(this IServiceCollection services)
        => services.AddSingleton<IVerenigingenRepository>(
            sp =>
                VerenigingenRepository.Load(sp.GetRequiredService<VerenigingenBlobClient>()).GetAwaiter().GetResult());

    public static void AddJsonLdContexts(this IServiceCollection services)
    {
        services.AddSingleton<IVerenigingenRepository>(
            sp =>
                VerenigingenRepository.Load(sp.GetRequiredService<VerenigingenBlobClient>()).GetAwaiter().GetResult());
    }

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

    public static void AddOpenTelemetry(this IServiceCollection services)
    {
        var assembly = typeof(Startup).Assembly;
        var serviceName = assembly.FullName;
        var serviceVersion = "1.0.0";
        var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
        Action<ResourceBuilder> configureResource = r => r.AddService(
                serviceName, serviceVersion: assemblyVersion, serviceInstanceId: Environment.MachineName)
            .AddAttributes(
                new Dictionary<string, object>()
                {
                    ["deployment.environment"] =
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant() ?? "unknown",
                });

        services.AddOpenTelemetryTracing(
            b =>
            {
                b
                    .AddSource(serviceName)
                    .ConfigureResource(configureResource).AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation(
                        options =>
                        {
                            options.EnrichWithHttpRequest =
                                (activity, request) => activity.SetParentId(request.Headers["traceparent"]);
                            options.Filter = context => context.Request.Method != HttpMethods.Options;
                        })
                    // .AddSqlClientInstrumentation(options => { options.SetDbStatementForText = true; })
                    .AddOtlpExporter(
                        options =>
                        {
                            options.Protocol = OtlpExportProtocol.Grpc;
                            options.Endpoint = new Uri("http://localhost:4317");
                        });
                // .AddConsoleExporter();
            });

        services.AddLogging(
            builder =>
            {
                builder.ClearProviders();
                builder.AddOpenTelemetry(
                    options =>
                    {
                        options.ConfigureResource(configureResource);

                        options.IncludeScopes = true;
                        options.IncludeFormattedMessage = true;
                        options.ParseStateValues = true;

                        options.AddOtlpExporter(
                                exporter =>
                                {
                                    exporter.Protocol = OtlpExportProtocol.Grpc;
                                    exporter.Endpoint = new Uri("http://localhost:4317");
                                })
                            .AddConsoleExporter();
                    });
            });

        services.AddOpenTelemetryMetrics(
            options =>
            {
                options.ConfigureResource(configureResource)
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                options.AddOtlpExporter(
                    exporter =>
                    {
                        exporter.Protocol = OtlpExportProtocol.Grpc;
                        exporter.Endpoint = new Uri("http://localhost:4317");
                    });
                // options.AddConsoleExporter();
            });
    }
}
