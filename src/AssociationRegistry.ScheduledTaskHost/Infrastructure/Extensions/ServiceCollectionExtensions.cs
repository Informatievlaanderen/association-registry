namespace AssociationRegistry.ScheduledTaskHost.Infrastructure.Extensions;

using Admin.Schema.Detail;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx.CodeGeneration;
using Marten;
using Marten.Events;
using Marten.Services;
using Newtonsoft.Json;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;
using Weasel.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenTelemetryServices(this IServiceCollection services)
    {
        var collectorUrl = CollectorUrl;
        var configureResource = ConfigureResource();

        services.AddOpenTelemetry()
                .ConfigureResource(configureResource)
                .WithTracing(builder =>
                 {
                     // Tracing

                     // Ensure the TracerProvider subscribes to any custom ActivitySources.
                     builder
                        .AddSource("Wolverine")
                        .SetSampler(new AlwaysOnSampler())
                        .AddHttpClientInstrumentation()
                        .AddNpgsql()
                        .AddOtlpExporter(
                             options =>
                             {
                                 options.Protocol = OtlpExportProtocol.Grpc;
                                 options.Endpoint = new Uri(collectorUrl);
                             });

                     builder.AddOtlpExporter(otlpOptions =>
                     {
                         otlpOptions.Protocol = OtlpExportProtocol.Grpc;

                         otlpOptions.Endpoint = new Uri(collectorUrl);
                     });
                 })
                .WithMetrics(builder =>
                 {
                     // Ensure the MeterProvider subscribes to any custom Meters.
                     builder
                        .AddRuntimeInstrumentation()
                        .AddHttpClientInstrumentation();

                     builder.AddOtlpExporter(otlpOptions =>
                     {
                         otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                         otlpOptions.Endpoint = new Uri(collectorUrl);
                     });
                 });

        return services;
    }

    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions)
    {
        services.AddSingleton(postgreSqlOptions);

        var martenConfiguration = services.AddMarten(
                                               serviceProvider =>
                                               {
                                                   var opts = new StoreOptions();
                                                   opts.Connection(postgreSqlOptions.GetConnectionString());
                                                   opts.Serializer(CreateMartenSerializer());
                                                   opts.Events.StreamIdentity = StreamIdentity.AsString;

                                                   opts.Events.MetadataConfig.EnableAll();
                                                   opts.AutoCreateSchemaObjects = AutoCreate.None;

                                                   opts.RegisterDocumentType<LocatieLookupDocument>();

                                                   opts.Schema.For<LocatieLookupDocument>()
                                                       .UseNumericRevisions(true)
                                                       .UseOptimisticConcurrency(false);

                                                   if (serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
                                                   {
                                                       opts.GeneratedCodeMode = TypeLoadMode.Dynamic;
                                                   }
                                                   else
                                                   {
                                                       opts.GeneratedCodeMode = TypeLoadMode.Auto;
                                                       opts.SourceCodeWritingEnabled = false;
                                                   }

                                                   return opts;
                                               })
                                          .UseLightweightSessions();



        return services;
    }

    private static JsonNetSerializer CreateMartenSerializer()
    {
        var jsonNetSerializer = new JsonNetSerializer();

        jsonNetSerializer.Customize(
            s =>
            {
                s.DateParseHandling = DateParseHandling.None;
            });

        return jsonNetSerializer;
    }

    public static string CollectorUrl
        => Environment.GetEnvironmentVariable("COLLECTOR_URL") ?? "http://localhost:4317";

    public static Action<ResourceBuilder> ConfigureResource()
    {
        var executingAssembly = Assembly.GetEntryAssembly()!;
        var serviceName = executingAssembly.GetName().Name!;
        var assemblyVersion = executingAssembly.GetName().Version?.ToString() ?? "unknown";

        Action<ResourceBuilder> configureResource = r => r
                                                        .AddService(
                                                             serviceName,
                                                             serviceVersion: assemblyVersion,
                                                             serviceInstanceId: Environment.MachineName)
                                                        .AddAttributes(
                                                             new Dictionary<string, object>
                                                             {
                                                                 ["deployment.environment"] =
                                                                     Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                                                                               ?.ToLowerInvariant()
                                                                  ?? "unknown",
                                                             });

        return configureResource;
    }
}
