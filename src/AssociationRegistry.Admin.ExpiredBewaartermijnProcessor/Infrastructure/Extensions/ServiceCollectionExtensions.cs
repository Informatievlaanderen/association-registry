namespace AssociationRegistry.Admin.ExpiredBewaartermijnProcessor.Infrastructure.Extensions;

using System.Reflection;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Admin.Schema.Persoonsgegevens;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Grar.NutsLau;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.MartenDb;
using AssociationRegistry.MartenDb.Setup;
using AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;
using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Resources;
using global::OpenTelemetry.Trace;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Npgsql;
using Wolverine.Marten;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenTelemetryServices(
        this IServiceCollection services)
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
                        .AddOtlpExporter(options =>
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

    public static string CollectorUrl
        => Environment.GetEnvironmentVariable("COLLECTOR_URL") ?? "http://localhost:4317";

    public static Action<ResourceBuilder> ConfigureResource()
    {
        var executingAssembly = Assembly.GetEntryAssembly()!;
        var serviceName = executingAssembly.GetName().Name!;
        var assemblyVersion = executingAssembly.GetName().Version?.ToString() ?? "unknown";

        Action<ResourceBuilder> configureResource = r => r
                                                        .AddService(
                                                             serviceName: serviceName,
                                                             serviceVersion: assemblyVersion,
                                                             serviceInstanceId: Environment.MachineName)
                                                        .AddAttributes(
                                                             new Dictionary<string, object>
                                                             {
                                                                 ["deployment.environment"] =
                                                                     Environment.GetEnvironmentVariable(
                                                                                     "ASPNETCORE_ENVIRONMENT")
                                                                               ?.ToLowerInvariant()
                                                                  ?? "unknown",
                                                             });

        return configureResource;
    }
}
