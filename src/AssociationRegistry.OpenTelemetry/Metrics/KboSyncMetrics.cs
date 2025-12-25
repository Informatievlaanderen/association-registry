namespace AssociationRegistry.OpenTelemetry.Metrics;

using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;

public class KboSyncMetrics
{
    public const string MeterName = "VR.Kbo.Sync.Handler";

    private readonly Counter<long> _lambdaInvocations;
    private readonly Counter<long> _syncOperations;
    private readonly Histogram<double> _syncDuration;
    private readonly string _environment;

    public KboSyncMetrics(Meter meter)
    {
        _environment = Environment.GetEnvironmentVariable("ENVIRONMENT")?.ToLowerInvariant() ?? "unknown";

        _lambdaInvocations = meter.CreateCounter<long>("kbo_sync_lambda_invocations_total", "Number of lambda invocations");
        _syncOperations = meter.CreateCounter<long>("kbo_sync_operations_total", "Number of sync operations");
        _syncDuration = meter.CreateHistogram<double>("kbo_sync_duration_seconds", "s", "Sync operation duration");
    }

    public void RecordLambdaInvocation(string lambdaName, bool coldStart)
    {
        var tags = new TagList
        {
            { "lambda_name", lambdaName },
            { "cold_start", coldStart },
            { "environment", _environment }
        };
        _lambdaInvocations.Add(1, tags);
    }

    public SyncScope Start(string syncType) => new SyncScope(this, syncType);

    internal void RecordSyncOperation(string syncType, string result, double durationSeconds)
    {
        var tags = new TagList
        {
            { "sync_type", syncType },
            { "result", result },
            { "environment", _environment }
        };
        _syncOperations.Add(1, tags);
        _syncDuration.Record(durationSeconds, tags);
    }

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
            _result = "failure"; // Default to failure unless explicitly marked as success/dropped
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
            _metrics.RecordSyncOperation(_syncType, _result, elapsedSec);
        }
    }
}
