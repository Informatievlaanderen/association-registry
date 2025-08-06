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
    public const string KboNumber = "kbo_number";
    public const string VCode = "vcode";
    public const string Result = "result";
}

public static class KboSyncMetrics
{
    public const string MeterName = "VR.Kbo.Sync.Handler";
    private const string MeterVersion = "1.0.0";
    private static readonly Meter Meter;

    public static readonly Counter<long> KboAttempts;
    public static readonly Counter<long> KboDrops;
    public static readonly UpDownCounter<long> KboInFlight;

    public static readonly Counter<long> Successes;
    public static readonly Counter<long> Failures;

    public static readonly Histogram<double> Duration;

    static KboSyncMetrics()
    {
        Meter = new Meter(MeterName, MeterVersion);

        KboAttempts = Meter.CreateCounter<long>("kbo_sync_attempts_total", "Sync attempts by KBO number");
        KboDrops = Meter.CreateCounter<long>("kbo_sync_drops_total", "Sync drops by KBO number");
        KboInFlight = Meter.CreateUpDownCounter<long>("kbo_sync_inflight", "In-flight syncs by KBO number");

        Successes = Meter.CreateCounter<long>("kbo_sync_vcode_success_total", "Sync successes by vCode");
        Failures = Meter.CreateCounter<long>("kbo_sync_vcode_failures_total", "Sync failures by vCode");

        Duration = Meter.CreateHistogram<double>("kbo_sync_duration_seconds", "s", "Sync duration");
    }

    public static SyncScope Start(string kboNumber) => new SyncScope(kboNumber);

    public struct SyncScope : IDisposable
    {
        private readonly string _kboNumber;
        private string? _vCode;
        private readonly long _startTimestamp;
        private bool _completed;

        public SyncScope(string kboNumber)
        {
            _kboNumber = kboNumber;
            _vCode = null;
            _startTimestamp = Stopwatch.GetTimestamp();
            _completed = false;

            var kboTags = new TagList([new(SyncLabels.KboNumber, _kboNumber)]);
            KboAttempts.Add(1, kboTags);
            KboInFlight.Add(1, kboTags);
        }

        public void Dropped()
        {
            _completed = true;

            var kboTags = new TagList([new(SyncLabels.KboNumber, _kboNumber)]);
            KboDrops.Add(1, kboTags);
        }

        public void Failed()
        {
            _completed = true;


            var vcodeTags = new TagList([new(SyncLabels.VCode, _vCode)]);
            Failures.Add(1, vcodeTags);
        }

        public void Succeed()
        {
            _completed = true;

            if (_vCode != null)
            {
                var vcodeTags = new TagList([new(SyncLabels.VCode, _vCode)]);
                Successes.Add(1, vcodeTags);
            }
        }

        public void UseVCode(string vCode)
        {
            _vCode = vCode;
        }

        public void Dispose()
        {
            try
            {
                var elapsedSec = (Stopwatch.GetTimestamp() - _startTimestamp) / (double)Stopwatch.Frequency;

                if (_vCode != null)
                {
                    var vcodeTags = new TagList([new(SyncLabels.VCode, _vCode)]);
                    Duration.Record(elapsedSec, vcodeTags);
                }
                else
                {
                    var kboTags = new TagList([new(SyncLabels.KboNumber, _kboNumber)]);
                    Duration.Record(elapsedSec, kboTags);
                }

                if (!_completed)
                {
                    Failed();
                }
            }
            finally
            {
                var kboTags = new TagList([new(SyncLabels.KboNumber, _kboNumber)]);
                KboInFlight.Add(-1, kboTags);
            }
        }
    }
}
