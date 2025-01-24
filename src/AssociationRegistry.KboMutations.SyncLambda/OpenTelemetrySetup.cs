namespace AssociationRegistry.KboMutations.SyncLambda;

using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

public class OpenTelemetrySetup
{
    private const string OtelAuthHeader = "OTEL_AUTH_HEADER";

    public static MeterProvider SetupMeter(string otlpMetricsUri)
    {
        return Sdk.CreateMeterProviderBuilder()
                  .AddMeter("KboMutations.SyncLambda.Metrics")
                  .AddRuntimeInstrumentation()
                  .AddHttpClientInstrumentation()
                  .AddConsoleExporter()
                  .AddOtlpExporter(options =>
                   {
                       options.Endpoint =
                           new Uri(otlpMetricsUri);

                       AddAuth(options);
                   })
                  .Build();
    }

    private static void AddAuth(OtlpExporterOptions options)
    {
        var authHeader = Environment.GetEnvironmentVariable(OtelAuthHeader);

        if (!string.IsNullOrEmpty(authHeader))
            options.Headers = $"Authorization={authHeader}";
    }

    public static TracerProvider SetUpTracing(string otlpTracingUri)
    {
        return Sdk.CreateTracerProviderBuilder()
                  .AddSource("KboMutations.SyncLambda.Tracing")
                  .AddAWSInstrumentation()// Optional: if Lambda is triggered by HTTP
                  .AddConsoleExporter()          // Replace with a production exporter
                  .AddOtlpExporter(options =>
                   {
                       options.Endpoint =
                           new Uri(otlpTracingUri);

                       AddAuth(options);
                   })
                  .Build();
    }

    public static void SetUpLogging(string? otlpLogssUri, ILoggingBuilder builder)
    {
        if (!string.IsNullOrEmpty(otlpLogssUri))
            builder.AddOpenTelemetry(options =>
            {
                options.AddConsoleExporter()
                       .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(otlpLogssUri);

                            AddAuth(options);
                        });
            });
    }
}
