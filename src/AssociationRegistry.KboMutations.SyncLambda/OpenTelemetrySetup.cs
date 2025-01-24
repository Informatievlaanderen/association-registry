namespace AssociationRegistry.KboMutations.SyncLambda;

using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

public class OpenTelemetrySetup : IDisposable
{
    private readonly OpenTelemetryResources _resources;
    private readonly Object _resource;
    private const string OtelAuthHeader = "OTEL_AUTH_HEADER";
    private const string MetricsUri = "OTLP_METRICS_URI";
    private const string TracingUri = "OTLP_TRACING_URI";
    private const string OtlpLogsUri = "OTLP_LOGS_URI";
    public static TracerProvider TracerProvider { get; }
    public static MeterProvider MeterProvider { get; }

    public const string MeterName = "KboMutations.SyncLambda.Metrics";

    public OpenTelemetrySetup()
    {
        _resources = GetResources();
    }

    public MeterProvider SetupMeter()
    {
        var otlpMetricsUri = Environment.GetEnvironmentVariable(MetricsUri);

        var builder = Sdk.CreateMeterProviderBuilder()
                         .ConfigureResource(_resources.ConfigureResourceBuilder)
                         .AddMeter(MeterName)
                         .AddRuntimeInstrumentation()
                         .AddHttpClientInstrumentation();

        if (!string.IsNullOrEmpty(otlpMetricsUri))
            builder
               .AddOtlpExporter(options =>
                {
                    options.Endpoint =
                        new Uri(otlpMetricsUri);

                    AddAuth(options);
                });

        return builder.Build();
    }

    public TracerProvider SetUpTracing()
    {
        var otlpTracingUri = Environment.GetEnvironmentVariable(TracingUri);

        var builder = Sdk.CreateTracerProviderBuilder()
                         .ConfigureResource(_resources.ConfigureResourceBuilder)
                         .AddSource("AssociationRegistry")
                         .AddConsoleExporter();

        if (!string.IsNullOrEmpty(otlpTracingUri))
            builder.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpTracingUri);
                AddAuth(options);
            });

        return builder.Build();
    }

    public void SetUpLogging(ILoggingBuilder builder)
    {
        var otlpLogssUri = Environment.GetEnvironmentVariable(OtlpLogsUri);

        builder.AddOpenTelemetry(options =>
        {
            var resourceBuilder = ResourceBuilder.CreateDefault();
            _resources.ConfigureResourceBuilder(resourceBuilder);
            options.SetResourceBuilder(resourceBuilder);

            if (!string.IsNullOrEmpty(otlpLogssUri))

                options.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpLogssUri);

                    AddAuth(options);
                });
        });
    }

    public OpenTelemetryResources GetResources()
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
                                                                     Environment.GetEnvironmentVariable("ENVIRONMENT")
                                                                               ?.ToLowerInvariant()
                                                                  ?? "unknown",
                                                             });

        return new OpenTelemetryResources(serviceName, configureResource);
    }

    public void Dispose()
    {
        MeterProvider.ForceFlush();
        MeterProvider.Dispose();

        TracerProvider.ForceFlush();
        TracerProvider.Dispose();
    }

    private void AddAuth(OtlpExporterOptions options)
    {
        var authHeader = Environment.GetEnvironmentVariable(OtelAuthHeader);

        if (!string.IsNullOrEmpty(authHeader))
            options.Headers = $"Authorization={authHeader}";
    }
}
public record OpenTelemetryResources(string ServiceName, Action<ResourceBuilder> ConfigureResourceBuilder);

