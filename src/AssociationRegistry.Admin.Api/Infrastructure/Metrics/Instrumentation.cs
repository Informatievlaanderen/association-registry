namespace AssociationRegistry.Admin.Api.Infrastructure.Metrics;

using OpenTelemetry;
using System.Diagnostics;
using System.Diagnostics.Metrics;

/// <summary>
/// It is recommended to use a custom type to hold references for
/// ActivitySource and Instruments. This avoids possible type collisions
/// with other components in the DI container.
/// </summary>
public class Instrumentation : IInstrumentation, IDisposable
{
    public string ActivitySourceName => "AssociationRegistry";
    public string MeterName => "AdminApi";
    private readonly Meter meter;

    public Instrumentation()
    {
        var version = typeof(Instrumentation).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        meter = new Meter(MeterName, version);

        _highWatermarkGauge = meter.CreateObservableGauge(name: "ar.beheer.p.highWatermark.g", unit: "events",
                                                          description: "high watermark", observeValue: () => HighWatermarkEventValue);
    }

    public ActivitySource ActivitySource { get; }
    private ObservableGauge<long> _highWatermarkGauge;
    public long HighWatermarkEventValue = 0;

    public void Dispose()
    {
        ActivitySource.Dispose();
        meter.Dispose();
    }
}
