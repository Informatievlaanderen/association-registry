namespace AssociationRegistry.KboMutations.SyncLambda.Telemetry;

using Amazon.Lambda.Core;
using global::OpenTelemetry;
using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Resources;
using global::OpenTelemetry.Trace;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using System.Reflection;

public class OpenTelemetrySetup : IDisposable
{
    private readonly OpenTelemetryResources _resources;
    private readonly ILambdaLogger _logger;
    public TracerProvider TracerProvider { get; private set; }
    public MeterProvider MeterProvider { get; private set; }

    public const string MeterName = "kbomutations.sync.lambda.metrics";

    public OpenTelemetrySetup(ILambdaLogger contextLogger, IConfigurationRoot configuration)
    {
        _logger = contextLogger;
        _logger.LogInformation("OpenTelemetrySetup: Starting setup");

        _resources = GetResources(contextLogger);


    }

    public MeterProvider SetupMeter(string? metricsUri, string? orgId)
    {
        var builder = Sdk.CreateMeterProviderBuilder()
                         .ConfigureResource(_resources.ConfigureResourceBuilder)
                         .AddMeter(MeterName)
                         .AddMeter(KboSyncMetrics.MeterName)
                         .AddMeter("Marten")
                         .AddRuntimeInstrumentation()
                         .AddHttpClientInstrumentation();

        if (!string.IsNullOrEmpty(metricsUri))
        {
            _logger.LogInformation($"Adding OTLP metrics exporter: {metricsUri}");
            builder.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(metricsUri);
                options.Protocol = OtlpExportProtocol.HttpProtobuf;

                AddHeaders(options, orgId);

                _logger.LogInformation($"Metrics - Endpoint: {options.Endpoint}");
                _logger.LogInformation($"Metrics - Protocol: {options.Protocol}");
                _logger.LogInformation($"Metrics - Headers: {options.Headers}");
            });
        }
        else
        {
            _logger.LogInformation("No metrics URI configured, skipping OTLP metrics exporter");
        }

        MeterProvider = builder.Build();

        return MeterProvider;
    }

    public TracerProvider SetUpTracing(string? tracesUri, string? orgId)
    {
        var builder = Sdk.CreateTracerProviderBuilder()
                         .AddHttpClientInstrumentation()
                         .AddNpgsql()
                         .ConfigureResource(_resources.ConfigureResourceBuilder);

        if (!string.IsNullOrEmpty(tracesUri))
        {
            _logger.LogInformation($"Adding OTLP traces exporter: {tracesUri}");
            builder.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(tracesUri);
                options.Protocol = OtlpExportProtocol.HttpProtobuf;

                AddHeaders(options, orgId);

                _logger.LogInformation($"Traces - Endpoint: {options.Endpoint}");
                _logger.LogInformation($"Traces - Protocol: {options.Protocol}");
                _logger.LogInformation($"Traces - Headers: {options.Headers}");
            });
        }
        else
        {
            _logger.LogInformation("No traces URI configured, skipping OTLP traces exporter");
        }

        TracerProvider = builder.Build();

        return TracerProvider;
    }

    public void SetUpLogging(string? logsUri, string? orgId, ILoggingBuilder builder)
    {
        builder.AddOpenTelemetry(options =>
        {
            var resourceBuilder = ResourceBuilder.CreateDefault();
            _resources.ConfigureResourceBuilder(resourceBuilder);
            options.SetResourceBuilder(resourceBuilder);

            if (!string.IsNullOrEmpty(logsUri))
            {
                _logger.LogInformation($"Adding OTLP logs exporter: {logsUri}");
                options.AddOtlpExporter(exporterOptions =>
                {
                    exporterOptions.Endpoint = new Uri(logsUri);
                    exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;

                    AddHeaders(exporterOptions, orgId);

                    _logger.LogInformation($"Logs - Endpoint: {exporterOptions.Endpoint}");
                    _logger.LogInformation($"Logs - Protocol: {exporterOptions.Protocol}");
                    _logger.LogInformation($"Logs - Headers: {exporterOptions.Headers}");
                });
            }
            else
            {
                _logger.LogInformation("No logs URI configured, skipping OTLP logs exporter");
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
        MeterProvider.Dispose();
        TracerProvider.Dispose();
    }

    private static void AddHeaders(OtlpExporterOptions options, string? orgScope)
    {
        var headersList = new List<string>();

        if (!string.IsNullOrEmpty(orgScope))
            headersList.Add($"X-Scope-OrgID={orgScope}");

        if (headersList.Any())
        {
            options.Headers = string.Join(",", headersList);
        }
        else
        {
            options.Headers = null;
        }
    }
}

public record OpenTelemetryResources(string ServiceName, Action<ResourceBuilder> ConfigureResourceBuilder);
