namespace AssociationRegistry.Acm.Api.Infrastructure.Metrics;

using OpenTelemetry;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;

/// <summary>
/// It is recommended to use a custom type to hold references for
/// ActivitySource and Instruments. This avoids possible type collisions
/// with other components in the DI container.
/// </summary>
public class AcmInstrumentation : IInstrumentation, IDisposable
{
    public string ActivitySourceName => "AssociationRegistry";
    public string MeterName => "AcmProjections";
    private readonly Meter meter;

    public AcmInstrumentation()
    {
        var version = typeof(AcmInstrumentation).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        meter = new Meter(MeterName, version);

        _verenigingPerInszGauge =
            meter.CreateObservableGauge(name: "ar.acm.p.verenigingPerInsz.g", unit: "events",
                                              description: "vereniging per insz projection",
                                              observeValue: () => VerenigingPerInszEventValue);
    }

    public ActivitySource ActivitySource { get; }
    private ObservableGauge<long> _verenigingPerInszGauge;

    public long VerenigingPerInszEventValue = 0;

    public void Dispose()
    {
        ActivitySource.Dispose();
        meter.Dispose();
    }
}
