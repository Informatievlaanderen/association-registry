namespace AssociationRegistry.OpenTelemetry.Extensions;

using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Resources;
using global::OpenTelemetry.Trace;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Reflection;

public static class ServiceCollectionExtensions
{
    private const string VrInitiatorHeaderName = "VR-Initiator";
    private const string XCorrelationIdHeaderName = "X-Correlation-Id";
    private const string BevestigingsTokenHeaderName = "VR-BevestigingsToken";

    public static IServiceCollection ConfigureOpenTelemetry<T>(this IHostApplicationBuilder builder, params T[] instrumentations)
        where T : class, IInstrumentation
    {
        var services = builder.Services;

        var (serviceName, collectorUrl, configureResource) = GetResources();

        foreach (var instrumentation in instrumentations)
        {
            services.AddSingleton(instrumentation);
        }

        builder.Logging.AddOpenTelemetry(logging =>
        {
            var resourceBuilder = ResourceBuilder
                                 .CreateDefault()
                                 .AddService(builder.Environment.ApplicationName);

            logging.SetResourceBuilder(resourceBuilder)
                   .AddOtlpExporter((options, processorOptions) =>
                    {
                        options.Endpoint = new Uri(collectorUrl);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
        });

        return services.AddOpenTelemetry()
                       .ConfigureResource(configureResource)
                       .WithMetrics(providerBuilder => providerBuilder
                                                      .AddRuntimeInstrumentation()
                                                      .AddAspNetCoreInstrumentation()
                                                      .AddHttpClientInstrumentation()
                                                      .AddOtlpExporter(options =>
                                                       {
                                                           options.Endpoint = new Uri(collectorUrl);
                                                           options.Protocol = OtlpExportProtocol.Grpc;
                                                       }))
                       .WithTracing(builder =>
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

                                                activity.SetCustomProperty(XCorrelationIdHeaderName,
                                                                           request.Headers[XCorrelationIdHeaderName]);

                                                activity.SetCustomProperty(BevestigingsTokenHeaderName,
                                                                           request.Headers[BevestigingsTokenHeaderName]);

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
                        })
                       .Services;
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
