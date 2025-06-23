using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Diagnostics.Metrics;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;

using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;

public enum SyncResult
{
    Attempt,
    Success,
    Failure,
    Dropped
}

public static class SyncLabels
{
    public const string VCode    = "vCode";
    public const string Result   = "result";
    public const string EntityId = "entity_id";  // used only for exemplars
}

public static class KboSyncMetrics
{
    public const string MeterName = "VR.Kbo.Sync.Handler";
    private const string MeterVersion = "1.0.0";
    private static readonly Meter Meter;

    public static readonly Counter<long> Attempts;
    public static readonly Counter<long> Successes;
    public static readonly Counter<long> Failures;
    public static readonly Histogram<double> Duration;
    public static readonly UpDownCounter<long> InFlight;

    static KboSyncMetrics()
    {
        Meter = new Meter(MeterName, MeterVersion);

        Attempts = Meter.CreateCounter<long>("kbo_sync_entities_attempt_total", "Sync attempts");
        Successes = Meter.CreateCounter<long>("kbo_sync_entities_success_total", "Sync successes");
        Failures = Meter.CreateCounter<long>("kbo_sync_entities_failure_total", "Sync failures");
        Duration = Meter.CreateHistogram<double>("kbo_sync_entities_duration_seconds", "s", "Sync duration");
        InFlight = Meter.CreateUpDownCounter<long>("kbo_sync_entities_inflight", "In-flight syncs");
    }

    public static SyncScope Start(string vCode) => new SyncScope(vCode);

    public struct SyncScope : IDisposable
    {
        private readonly string _initialId; // KBO nummer
        private string _currentId; // switches to vCode later
        private readonly long _startTimestamp;
        private bool _completed;

    public SyncScope(string kboNummer)
    {
        _initialId      = kboNummer;
        _currentId      = kboNummer;
        _startTimestamp = Stopwatch.GetTimestamp();
        _completed      = false;

        // 1) record the attempt with KBO as exemplar
        var tags = BuildTags(_currentId, SyncResult.Attempt, exemplar: _initialId);
        KboSyncMetrics.Attempts.Add(1, tags);
        KboSyncMetrics.InFlight.Add(1, tags);
    }

    /// <summary>Call if you exit early because the record doesn’t exist.</summary>
    public void Dropped()
    {
        _completed = true;
        var tags = BuildTags(_currentId, SyncResult.Dropped, exemplar: _initialId);
        // We count drops as failures in the Failures counter, but distinguish by label
        KboSyncMetrics.Failures.Add(1, tags);
    }

    /// <summary>Call for other failures (e.g. exceptions).</summary>
    public void Failed()
    {
        _completed = true;
        var tags = BuildTags(_currentId, SyncResult.Failure, exemplar: _currentId);
        KboSyncMetrics.Failures.Add(1, tags);
    }

    /// <summary>Call once the sync fully succeeded.</summary>
    public void Succeed()
    {
        _completed = true;
        var tags = BuildTags(_currentId, SyncResult.Success, exemplar: _currentId);
        KboSyncMetrics.Successes.Add(1, tags);
    }

    /// <summary>Switch the exemplar from KBO nummer to the loaded vCode.</summary>
    public void UseVCode(string vCode)
    {
        _currentId = vCode;
    }

    public void Dispose()
    {
        // 4) record duration with whichever ID is current
        var elapsedSec = (Stopwatch.GetTimestamp() - _startTimestamp)
                         / (double)Stopwatch.Frequency;
        KboSyncMetrics.Duration.Record(
            elapsedSec,
            BuildTags(_currentId, exemplar: _currentId));

        // 5) if you never marked completion, count it as a failure
        if (!_completed)
        {
            Failed();
        }

        // 6) decrement in-flight
        KboSyncMetrics.InFlight.Add(-1,
            BuildTags(_currentId, exemplar: _currentId));
    }

        private static TagList BuildTags(
            string id,
            SyncResult? result = null,
            string? exemplar = null)
        {
            var tags = new TagList(
            [
                new(SyncLabels.VCode, id)
            ]);

            if (result.HasValue)
            {
                tags.Add(new(SyncLabels.Result,
                                 result.Value.ToString().ToLowerInvariant()));
            }

            if (!string.IsNullOrEmpty(exemplar))
            {
                tags.Add(new(SyncLabels.EntityId, exemplar));
            }

            return new TagList(tags.ToArray());
        }
    }
}
