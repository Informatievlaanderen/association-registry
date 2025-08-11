namespace AssociationRegistry.Admin.AddressSync.Infrastructure.Extensions;

using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Resources;
using global::OpenTelemetry.Trace;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using Marten;
using Marten.Events;
using Marten.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Npgsql;
using Schema.Detail;
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
                                                   opts.UseNewtonsoftForSerialization(configure: settings =>
                                                   {
                                                       settings.DateParseHandling = DateParseHandling.None;
                                                       // TODO: use common marten project
                                                       // settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                                                       // settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
                                                   });
                                                   opts.Events.StreamIdentity = StreamIdentity.AsString;

                                                   opts.Events.MetadataConfig.EnableAll();
                                                   opts.AutoCreateSchemaObjects = AutoCreate.None;

                                                   opts.RegisterDocumentType<LocatieLookupDocument>();

                                                   opts.Schema.For<LocatieLookupDocument>().UseNumericRevisions(true)
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
