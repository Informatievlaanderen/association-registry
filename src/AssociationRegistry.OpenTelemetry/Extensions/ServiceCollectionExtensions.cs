namespace AssociationRegistry.OpenTelemetry.Extensions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Destructurama;
using global::OpenTelemetry;
using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Resources;
using global::OpenTelemetry.Trace;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;
using System.Reflection;

public static class ServiceCollectionExtensions
{
    private const string VrInitiatorHeaderName = "VR-Initiator";
    private const string XCorrelationIdHeaderName = "X-Correlation-Id";
    private const string BevestigingsTokenHeaderName = "VR-BevestigingsToken";

    private static ILoggingBuilder ConfigureOpenTelemetryLogging(this IHostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        var loggerConfig =
            new LoggerConfiguration()
               .Destructure.JsonNetTypes()
               .ReadFrom.Configuration(builder.Configuration)
               .Enrich.FromLogContext()
               .Enrich.WithMachineName()
               .Enrich.WithThreadId()
               .Enrich.WithEnvironmentUserName()
               .MinimumLevel.Information()
               .Filter.ByExcluding(e => e is { Exception: DomainException, Level: LogEventLevel.Error })
               .WriteTo.Logger(lc => lc
                                    .Filter.ByIncludingOnly(evt => evt.Exception is DomainException)
                                    .MinimumLevel.Information()
                                    .WriteTo.OpenTelemetry(ConfigureOpenTelemetry)
                )
               .WriteTo.OpenTelemetry(ConfigureOpenTelemetry);
        Log.Logger = loggerConfig.CreateLogger();
        return builder.Logging.AddSerilog();
    }

    private static void ConfigureOpenTelemetry(BatchedOpenTelemetrySinkOptions options)
    {
        var (serviceName, collectorUrl, configureResource) = ServiceCollectionExtensions.GetResources();
        options.Endpoint = collectorUrl;
        options.Protocol = OtlpProtocol.Grpc;
        var executingAssembly = Assembly.GetEntryAssembly()!;
        var assemblyVersion = executingAssembly.GetName().Version?.ToString() ?? "unknown";

        options.IncludedData = IncludedData.MessageTemplateTextAttribute | IncludedData.TraceIdField | IncludedData.SpanIdField | IncludedData.SourceContextAttribute;

        options.HttpMessageHandler = new SocketsHttpHandler
        {
            ActivityHeadersPropagator = null
        };

        options.BatchingOptions.BatchSizeLimit = 700;
        options.BatchingOptions.Period = TimeSpan.FromSeconds(1);
        options.BatchingOptions.QueueLimit = 10;

        options.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = serviceName,
            ["service.version"] = assemblyVersion,
            ["service.instanceId"] = Environment.MachineName,
            ["service.name"] = serviceName,

            ["deployment.environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                                                   ?.ToLowerInvariant() ?? "unknown",
        };
    }

    public static IServiceCollection ConfigureOpenTelemetry<T>(this IHostApplicationBuilder builder, params T[] instrumentations)
        where T : class, IInstrumentation
    {
        var services = builder.Services;

        var (serviceName, collectorUrl, configureResource) = GetResources();

        foreach (var instrumentation in instrumentations)
        {
            services.AddSingleton(instrumentation);
        }

        builder.ConfigureOpenTelemetryLogging();
        return services.AddOpenTelemetry()
                       .ConfigureResource(configureResource)
                       .WithMetrics(providerBuilder => providerBuilder
                                                      .ConfigureResource(configureResource)
                                                      .AddMeter($"Wolverine:{serviceName}")
                                                      .AddMeter("Marten")
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

    public static (string serviceName, string collectorUrl, Action<ResourceBuilder> configureResource) GetResources()
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
