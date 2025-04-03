namespace AssociationRegistry.KboMutations.SyncLambda;

using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
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
    private OtlpConfigs _otlpConfigs;
    private const string OtelAuthHeader = "OTEL_AUTH_HEADER";
    private const string MetricsUri = "OTLP_METRICS_URI";
    private const string TracingUri = "OTLP_TRACING_URI";
    private const string OtlpLogsUri = "OTLP_LOGS_URI";
    public TracerProvider TracerProvider { get; }
    public MeterProvider MeterProvider { get; }

    public const string MeterName = "kbomutations.sync.lambda.metrics";

    public OpenTelemetrySetup(ILambdaLogger contextLogger, IConfigurationRoot configuration)
    {
        _otlpConfigs = new OtlpConfigs();
        configuration.Bind(_otlpConfigs.Configs);

        _resources = GetResources(contextLogger);

        MeterProvider = SetupMeter();
        TracerProvider = SetUpTracing();
    }

    public MeterProvider SetupMeter()
    {
        var builder = Sdk.CreateMeterProviderBuilder()
                         .ConfigureResource(_resources.ConfigureResourceBuilder)
                         .AddMeter(MeterName)
                         .AddMeter("Marten")
                         .AddConsoleExporter()
                         .AddRuntimeInstrumentation()
                         .AddHttpClientInstrumentation();

        foreach (var otlpConfig in _otlpConfigs.Configs)
        {
            if (!string.IsNullOrEmpty(otlpConfig.Value.MetricsUri))
                builder.AddOtlpExporter(options =>
                {
                    options.Endpoint =
                        new Uri(otlpConfig.Value.MetricsUri);
                    AddAuth(options, otlpConfig.Value.AuthHeader);
                    AddOrgScope(options, otlpConfig.Value.OrgId);
                });
        }

        return builder.Build();
    }

    public TracerProvider SetUpTracing()
    {
        var builder = Sdk.CreateTracerProviderBuilder()
                         .AddHttpClientInstrumentation()
                         .AddNpgsql()
                         .ConfigureResource(_resources.ConfigureResourceBuilder)
                         .AddConsoleExporter();

        foreach (var otlpConfig in _otlpConfigs.Configs)
        {
            if (!string.IsNullOrEmpty(otlpConfig.Value.TracingUri))
                builder.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpConfig.Value.TracingUri);
                    AddAuth(options, otlpConfig.Value.AuthHeader);
                });
        }

        return builder.Build();
    }

    public void SetUpLogging(ILoggingBuilder builder)
    {
        builder.AddOpenTelemetry(options =>
        {
            var resourceBuilder = ResourceBuilder.CreateDefault();
            _resources.ConfigureResourceBuilder(resourceBuilder);
            options.SetResourceBuilder(resourceBuilder);

            foreach (var otlpConfig in _otlpConfigs.Configs)
            {
                if (!string.IsNullOrEmpty(otlpConfig.Value.LogsUri))
                    options.AddOtlpExporter(exporterOptions =>
                    {
                        exporterOptions.Endpoint = new Uri(otlpConfig.Value.LogsUri);

                        AddAuth(exporterOptions, otlpConfig.Value.AuthHeader);
                    });
            }
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

    private static void AddAuth(OtlpExporterOptions options, string authHeader)
    {
        if (!string.IsNullOrEmpty(authHeader))
            options.Headers = $"Authorization={authHeader}";
    }

    private static void AddOrgScope(OtlpExporterOptions options, string orgScope)
    {
        if (!string.IsNullOrEmpty(orgScope))
            options.Headers = $"X-Scope-OrgID={orgScope}";
    }
}
public record OpenTelemetryResources(string ServiceName, Action<ResourceBuilder> ConfigureResourceBuilder);

