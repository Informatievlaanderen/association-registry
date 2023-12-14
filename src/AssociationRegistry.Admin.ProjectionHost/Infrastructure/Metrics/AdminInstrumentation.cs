namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Metrics;

using AssociationRegistry.OpenTelemetry;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;

/// <summary>
/// It is recommended to use a custom type to hold references for
/// ActivitySource and Instruments. This avoids possible type collisions
/// with other components in the DI container.
/// </summary>
public class AdminInstrumentation : IInstrumentation, IDisposable
{
    public string ActivitySourceName => "AssociationRegistry";
    public string MeterName => "AdminProjections";
    private readonly Meter meter;

    public AdminInstrumentation()
    {
        var version = typeof(AdminInstrumentation).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        meter = new Meter(MeterName, version);

        _verenigingZoeken = meter.CreateObservableGauge(name: "ar.beheer.p.zoeken.g", unit: "events", description: "Beheer zoeken projection", observeValue:() => VerenigingZoekenEventValue);
        _verenigingDetail = meter.CreateObservableGauge(name: "ar.beheer.p.detail.g", unit: "events", description: "Beheer detail projection", observeValue:() => VerenigingDetailEventValue);
        _historiek = meter.CreateObservableGauge(name: "ar.beheer.p.historiek.g", unit: "events", description: "Beheer detail projection", observeValue:() => VerenigingHistoriekEventValue);
    }
    public ActivitySource ActivitySource { get; }

    private ObservableGauge<long> _verenigingZoeken;
    public long VerenigingZoekenEventValue { get; set; }
    private ObservableGauge<long> _verenigingDetail;
    public long VerenigingDetailEventValue { get; set; }

    private ObservableGauge<long> _historiek;
    public long VerenigingHistoriekEventValue { get; set; }


    public void Dispose()
    {
        ActivitySource.Dispose();
        meter.Dispose();
    }
}
