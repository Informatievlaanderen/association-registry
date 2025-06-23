namespace AssociationRegistry.KboMutations.SyncLambda.Telemetry;

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
                         .AddMeter(KboSyncMetrics.MeterName)
                         .AddMeter("Marten")
                         .AddConsoleExporter()
                         .AddRuntimeInstrumentation()
                         .AddHttpClientInstrumentation();

        foreach (var otlpConfig in _otlpConfigs.Configs)
        {
            if (!string.IsNullOrEmpty(otlpConfig.Value.MetricsUri))
                builder.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpConfig.Value.MetricsUri);
                    AddHeaders(options, otlpConfig.Value.AuthHeader, Environment.GetEnvironmentVariable($"OTLP_{otlpConfig.Key.ToUpper()}__ORGID"));
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
                    AddHeaders(options, otlpConfig.Value.AuthHeader, Environment.GetEnvironmentVariable($"OTLP_{otlpConfig.Key.ToUpper()}__ORGID"));
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
                        AddHeaders(exporterOptions, otlpConfig.Value.AuthHeader, Environment.GetEnvironmentVariable($"OTLP_{otlpConfig.Key.ToUpper()}__ORGID"));
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

        return new OpenTelemetryResources(serviceName, configureResource);
    }

    public void Dispose()
    {
        // Note: ForceFlush now handled in Lambda's FlushAllTelemetryAsync method
        // This is just for cleanup
        MeterProvider.Dispose();
        TracerProvider.Dispose();
    }

    private static void AddHeaders(OtlpExporterOptions options, string authHeader, string orgScope)
    {
        var headersList = new List<string>();

        if (!string.IsNullOrEmpty(authHeader))
            headersList.Add($"Authorization={authHeader}");

        if (!string.IsNullOrEmpty(orgScope))
            headersList.Add($"X-Scope-OrgID={orgScope}");

        if (headersList.Any())
            options.Headers = string.Join(",", headersList);
    }
}

public record OpenTelemetryResources(string ServiceName, Action<ResourceBuilder> ConfigureResourceBuilder);
