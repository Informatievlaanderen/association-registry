namespace AssociationRegistry.KboMutations.SyncLambda.Telemetry;

using Amazon.Lambda.Core;
using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

public class TelemetryManager : IDisposable
{
    private OpenTelemetrySetup? _openTelemetrySetup;
    private readonly ILambdaLogger _logger;

    private readonly string? _logsUri;
    private readonly string? _orgId;
    private readonly string? _metricsUri;
    private readonly string? _tracesUri;

    public TelemetryManager(ILambdaLogger logger, IConfigurationRoot configuration)
    {
        _logger = logger;

        _metricsUri = configuration["OTLP_METRICS_URI"];
        _tracesUri = configuration["OTLP_TRACES_URI"];
        _logsUri = configuration["OTLP_LOGS_URI"];
        _orgId = configuration["OTLP_ORG_ID"];

        _logger.LogInformation($"OTLP config - Metrics URI: {_metricsUri}");
        _logger.LogInformation($"OTLP config - Traces URI: {_tracesUri}");
        _logger.LogInformation($"OTLP config - Logs URI: {_logsUri}");
        _logger.LogInformation($"OTLP config - Org ID: {_orgId}");

        _openTelemetrySetup = new OpenTelemetrySetup(logger, configuration);
        _openTelemetrySetup.SetupMeter(_metricsUri, _orgId);
        _openTelemetrySetup.SetUpTracing(_tracesUri, _orgId);
    }

    public void ConfigureLogging(ILoggingBuilder builder)
    {
        _openTelemetrySetup?.SetUpLogging(_logsUri, _orgId, builder);
    }

    public async Task FlushAsync(ILambdaContext context)
    {
        if (_openTelemetrySetup == null) return;

        try
        {
            var remainingTime = context.RemainingTime.Subtract(5.Seconds());
            var maxFlushTime = TimeSpan.FromMilliseconds(Math.Min(remainingTime.TotalMilliseconds, 5.Seconds().Milliseconds));

            _logger.LogInformation($"Flushing OpenTelemetry data with timeout: {maxFlushTime.TotalMilliseconds}ms");

            using var cts = new CancellationTokenSource(maxFlushTime);

            await Task.Run(() =>
            {
                var metricsFlushResult = _openTelemetrySetup.MeterProvider.ForceFlush();
                _logger.LogInformation($"Metrics flush result: {metricsFlushResult}");

                var tracesFlushResult = _openTelemetrySetup.TracerProvider.ForceFlush();
                _logger.LogInformation($"Traces flush result: {tracesFlushResult}");

            }, cts.Token);

            _logger.LogInformation("OpenTelemetry flush completed successfully");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("OpenTelemetry flush operation timed out");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during OpenTelemetry flush");
        }
    }

    public void Dispose()
    {
        try
        {
            _openTelemetrySetup?.Dispose();
            _openTelemetrySetup = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing OpenTelemetry setup");
        }
    }
}
