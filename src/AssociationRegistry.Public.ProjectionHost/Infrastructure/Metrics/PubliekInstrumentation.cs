namespace AssociationRegistry.Public.ProjectionHost.Metrics;

using OpenTelemetry;
using System.Diagnostics;
using System.Diagnostics.Metrics;

/// <summary>
/// It is recommended to use a custom type to hold references for
/// ActivitySource and Instruments. This avoids possible type collisions
/// with other components in the DI container.
/// </summary>
public class PubliekInstrumentation : IInstrumentation, IDisposable
{
    public string ActivitySourceName => "AssociationRegistry";
    public string MeterName => "PubliekProjections";
    private readonly Meter meter;

    public PubliekInstrumentation()
    {
        var version = typeof(PubliekInstrumentation).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        meter = new Meter(MeterName, version);

        _verenigingZoeken = meter.CreateObservableGauge(name: "ar.publiek.p.zoeken.g", unit: "events",
                                                        description: "Beheer zoeken projection",
                                                        observeValue: () => VerenigingZoekenEventValue);

        _verenigingDetail = meter.CreateObservableGauge(name: "ar.publiek.p.detail.g", unit: "events",
                                                        description: "Beheer detail projection",
                                                        observeValue: () => VerenigingDetailEventValue);
    }

    private ObservableGauge<long> _verenigingDetail;
    public long VerenigingDetailEventValue { get; set; }
    private ObservableGauge<long> _verenigingZoeken;
    public long VerenigingZoekenEventValue { get; set; }
    public ActivitySource ActivitySource { get; }

    public void Dispose()
    {
        ActivitySource.Dispose();
        meter.Dispose();
    }
}
