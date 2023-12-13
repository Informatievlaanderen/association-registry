namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Metrics;

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
    public string MeterName => "PublicApi";
    private readonly Meter meter;

    public Instrumentation()
    {
        var version = typeof(Instrumentation).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        meter = new Meter(MeterName, version);

        PubliekVerenigingDetailHistogram = meter.CreateHistogram<long>(name: "ar.p.publiekVerenigingDetail.h", unit: "events",
                                                                       description: "publiek vereniging detail histogram");

        PubliekVerenigingZoekenHistogram =
            meter.CreateHistogram<long>(name: "ar.p.publiekVerenigingZoeken.h", unit: "events", description: "publiek vereniging zoeken histogram");
    }

    public ActivitySource ActivitySource { get; }
    public Histogram<long> PubliekVerenigingDetailHistogram { get; }
    public Histogram<long> PubliekVerenigingZoekenHistogram { get; }

    public void Dispose()
    {
        ActivitySource.Dispose();
        meter.Dispose();
    }
}
