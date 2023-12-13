namespace AssociationRegistry.OpenTelemetry.Extensions;

using System.Diagnostics;
using System.Diagnostics.Metrics;

/// <summary>
/// It is recommended to use a custom type to hold references for
/// ActivitySource and Instruments. This avoids possible type collisions
/// with other components in the DI container.
/// </summary>
public class Instrumentation : IDisposable
{
    internal const string ActivitySourceName = "AssociationRegistry";
    internal const string MeterName = "AdminApi";
    private readonly Meter meter;

    public Instrumentation()
    {
        var version = typeof(Instrumentation).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        meter = new Meter(MeterName, version);

        HighWaterMarkHistogram = meter.CreateHistogram<long>(name: "ar.highWatermark.h", unit: "events", description: "high watermark");

        HighWaterMarkGauge = meter.CreateObservableGauge(name: "ar.highWatermark.g",
                                                         observeValue: () => HighWaterMark);

        HighWaterMarkCounter = meter.CreateObservableUpDownCounter(name: "ar.highWatermark.c",
                                                                   observeValue: () => new Measurement<long>(HighWaterMark));
    }

    public ObservableUpDownCounter<long> HighWaterMarkCounter { get; set; }
    public ObservableGauge<long> HighWaterMarkGauge { get; set; }
    public ActivitySource ActivitySource { get; }
    public Histogram<long> HighWaterMarkHistogram { get; }
    public long HighWaterMark { get; set; }

    public void Dispose()
    {
        ActivitySource.Dispose();
        meter.Dispose();
    }
}
