namespace AssociationRegistry.OpenTelemetry.Metrics;

using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;

public class KboSyncMetrics
{
    public const string MeterName = "VR.Kbo.Sync.Handler";
    private readonly Meter _meter;
    private readonly string _environment;
    private readonly ObservableGauge<long> _filesProcessed;
    private readonly Histogram<long> _invocationTimestamp;
    private readonly Histogram<double> _syncDuration;
    private long _filesProcessedThisInvocation;

    public KboSyncMetrics(Meter meter)
    {
        _meter = meter;
        _environment = System.Environment.GetEnvironmentVariable("ENVIRONMENT")?.ToLowerInvariant() ?? "unknown";

        _filesProcessed = _meter.CreateObservableGauge(
            name: "kbo_sync_files_processed",
            observeValue: () => new Measurement<long>(_filesProcessedThisInvocation, new KeyValuePair<string, object?>("environment", _environment)),
            description: "Number of files processed in current invocation"
        );

        _invocationTimestamp = _meter.CreateHistogram<long>(
            name: "kbo_sync_lambda_invocation_timestamp",
            unit: "ms",
            description: "Lambda invocation timestamp"
        );

        _syncDuration = _meter.CreateHistogram<double>(
            name: "kbo_sync_duration_seconds",
            unit: "s",
            description: "Sync operation duration"
        );
    }

    public void RecordLambdaInvocation(string lambdaName, bool coldStart)
    {
        var tags = new TagList
        {
            { "lambda_name", lambdaName },
            { "cold_start", coldStart },
            { "environment", _environment }
        };
        _invocationTimestamp.Record(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), tags);
    }

    public void RecordFilesProcessed(long count)
    {
        _filesProcessedThisInvocation = count;
    }

    public SyncScope Start(string syncType) => new(this, syncType);

    public struct SyncScope : IDisposable
    {
        private readonly KboSyncMetrics _metrics;
        private readonly string _syncType;
        private readonly long _startTimestamp;
        private string _result;

        public SyncScope(KboSyncMetrics metrics, string syncType)
        {
            _metrics = metrics;
            _syncType = syncType;
            _startTimestamp = Stopwatch.GetTimestamp();
            _result = "failure";
        }

        public void Dropped()
        {
            _result = "dropped";
        }

        public void Failed()
        {
            _result = "failure";
        }

        public void Succeed()
        {
            _result = "success";
        }

        public void Dispose()
        {
            var elapsedSec = (Stopwatch.GetTimestamp() - _startTimestamp) / (double)Stopwatch.Frequency;
            var tags = new TagList
            {
                { "sync_type", _syncType },
                { "result", _result },
                { "environment", _metrics._environment }
            };
            _metrics._syncDuration.Record(elapsedSec, tags);
        }
    }
}
