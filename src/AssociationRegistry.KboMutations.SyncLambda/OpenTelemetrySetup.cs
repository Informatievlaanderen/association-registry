namespace AssociationRegistry.KboMutations.SyncLambda;

using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;
using Npgsql;
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
    private const string OtelAuthHeader = "OTEL_AUTH_HEADER";
    private const string MetricsUri = "OTLP_METRICS_URI";
    private const string TracingUri = "OTLP_TRACING_URI";
    private const string OtlpLogsUri = "OTLP_LOGS_URI";
    public TracerProvider TracerProvider { get; }
    public MeterProvider MeterProvider { get; }

    public const string MeterName = "kbomutations.sync.lambda.metrics";

    public OpenTelemetrySetup(ILambdaLogger contextLogger)
    {
        _resources = GetResources(contextLogger);

        MeterProvider = SetupMeter();
        TracerProvider = SetUpTracing();
    }

    public MeterProvider SetupMeter()
    {
        var otlpMetricsUri = Environment.GetEnvironmentVariable(MetricsUri);

        var builder = Sdk.CreateMeterProviderBuilder()
                         .ConfigureResource(_resources.ConfigureResourceBuilder)
                         .AddMeter(MeterName)
                         .AddMeter("Marten")
                         .AddConsoleExporter()
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
                         .AddHttpClientInstrumentation()
                         .AddNpgsql()
                         .ConfigureResource(_resources.ConfigureResourceBuilder)
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

                options.AddOtlpExporter(exporterOptions =>
                {
                    exporterOptions.Endpoint = new Uri(otlpLogssUri);

                    AddAuth(exporterOptions);
                });
        });
    }

    public OpenTelemetryResources GetResources(ILambdaLogger contextLogger)
    {
        var serviceName = "KboMutations.SyncLambda";
        var assemblyVersion = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "unknown";
        var serviceInstanceId = Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME") ?? Environment.MachineName;
        var environment = Environment.GetEnvironmentVariable("ENVIRONMENT")?.ToLowerInvariant() ?? "unknown";

        Action<ResourceBuilder> configureResource = r =>
        {
            r
               .AddService(
                    serviceName,
                    serviceVersion: assemblyVersion,
                    serviceInstanceId: serviceInstanceId)
               .AddAttributes(
                    new Dictionary<string, object>
                    {
                        ["deployment.environment"] = environment,
                    });
        };

        contextLogger.LogInformation("Resource configuration: " +
                                     "Service name '{ServiceName}', " +
                                     "ServiceVersion '{AssemblyVersion}', " +
                                     "Service Instance Id '{ServiceInstanceId}', " +
                                     "Env '{Env}'",
                                     serviceName, assemblyVersion, serviceInstanceId, environment);

        return  new OpenTelemetryResources(serviceName, configureResource);
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

