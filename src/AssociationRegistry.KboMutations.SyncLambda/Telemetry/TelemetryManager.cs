namespace AssociationRegistry.KboMutations.SyncLambda.Telemetry;

using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

public class TelemetryManager : IDisposable
{
    private OpenTelemetrySetup? _openTelemetrySetup;
    private readonly ILambdaLogger _logger;

    public TelemetryManager(ILambdaLogger logger, IConfigurationRoot configuration)
    {
        _logger = logger;
        _openTelemetrySetup = new OpenTelemetrySetup(logger, configuration);
    }

    public void ConfigureLogging(ILoggingBuilder builder)
    {
        _openTelemetrySetup?.SetUpLogging(builder);
    }

    public async Task FlushAsync(ILambdaContext context)
    {
        if (_openTelemetrySetup == null) return;

        try
        {
            var remainingTime = context.RemainingTime.Subtract(TimeSpan.FromMilliseconds(500));
            var maxFlushTime = TimeSpan.FromMilliseconds(Math.Min(remainingTime.TotalMilliseconds, 5000));

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
