namespace AssociationRegistry.PowerBi.ExportHost.Infrastructure.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

public static class ServiceCollectionOpenTelemetryExtensions
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
