namespace AssociationRegistry.OpenTelemetry.Extensions;

using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Resources;
using global::OpenTelemetry.Trace;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Reflection;

public static class ServiceCollectionExtensions
{
    private const string VrInitiatorHeaderName = "VR-Initiator";

    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services)
    {
        var (serviceName, collectorUrl, configureResource) = GetResources();

        return services.AddTracking(configureResource, collectorUrl, serviceName)
                       .AddLogging(configureResource, collectorUrl)
                       .AddMetrics(configureResource, collectorUrl);
    }

    public static IServiceCollection AddOpenTelemetry<T>(this IServiceCollection services, params T[] instrumentations)
        where T : class, IInstrumentation
    {
        var (serviceName, collectorUrl, configureResource) = GetResources();

        foreach (var instrumentation in instrumentations)
        {
            services.AddSingleton(instrumentation);
        }

        return services.AddTracking(configureResource, collectorUrl, serviceName)
                       .AddLogging(configureResource, collectorUrl)
                       .AddMetrics(configureResource, collectorUrl, extraConfig: builder =>
                        {
                            foreach (var instrumentation in instrumentations)
                            {
                                builder.AddMeter(instrumentation.MeterName);
                            }
                        });
    }

    private static IServiceCollection AddMetrics(
        this IServiceCollection services,
        Action<ResourceBuilder> configureResource,
        string collectorUrl,
        Action<MeterProviderBuilder>? extraConfig = null)
    {
        return services.AddOpenTelemetryMetrics(
            options =>
            {
                options
                   .ConfigureResource(configureResource)
                   .AddRuntimeInstrumentation()
                   .AddHttpClientInstrumentation()
                   .AddAspNetCoreInstrumentation()
                   .AddOtlpExporter(
                        exporter =>
                        {
                            exporter.Protocol = OtlpExportProtocol.Grpc;
                            exporter.Endpoint = new Uri(collectorUrl);
                        })
                   .InvokeIfNotNull(extraConfig);
            });
    }

    private static T InvokeIfNotNull<T>(this T obj, Action<T>? method)
    {
        method?.Invoke(obj);

        return obj;
    }

    private static IServiceCollection AddLogging(
        this IServiceCollection services,
        Action<ResourceBuilder> configureResource,
        string collectorUrl)
    {
        return services.AddLogging(
            builder =>
                builder
                   .AddOpenTelemetry(
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
                                    exporter.Endpoint = new Uri(collectorUrl);
                                });
                        }));
    }

    private static IServiceCollection AddTracking(
        this IServiceCollection services,
        Action<ResourceBuilder> configureResource,
        string collectorUrl,
        string serviceName)
    {
        return services.AddOpenTelemetryTracing(
            builder =>
            {
                builder
                   .AddSource(serviceName)
                   .ConfigureResource(configureResource).AddHttpClientInstrumentation()
                   .AddAspNetCoreInstrumentation(
                        options =>
                        {
                            options.EnrichWithHttpRequest =
                                (activity, request) =>
                                {
                                    activity.SetCustomProperty(VrInitiatorHeaderName, request.Headers[VrInitiatorHeaderName]);
                                    activity.SetParentId(request.Headers["traceparent"]);
                                };

                            options.Filter = context => context.Request.Method != HttpMethods.Options;
                        })
                   .AddNpgsql()
                   .AddOtlpExporter(
                        options =>
                        {
                            options.Protocol = OtlpExportProtocol.Grpc;
                            options.Endpoint = new Uri(collectorUrl);
                        })
                   .AddSource("Wolverine");
            });
    }

    private static (string serviceName, string collectorUrl, Action<ResourceBuilder> configureResource) GetResources()
    {
        var executingAssembly = Assembly.GetEntryAssembly()!;
        var serviceName = executingAssembly.GetName().Name!;
        var assemblyVersion = executingAssembly.GetName().Version?.ToString() ?? "unknown";
        var collectorUrl = Environment.GetEnvironmentVariable("COLLECTOR_URL") ?? "http://localhost:4317";

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

        return (serviceName, collectorUrl, configureResource);
    }
}
