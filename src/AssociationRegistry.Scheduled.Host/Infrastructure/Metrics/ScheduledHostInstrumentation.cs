namespace AssociationRegistry.Scheduled.Host.Infrastructure.Metrics;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry;

public class ScheduledHostInstrumentation : IInstrumentation, IDisposable
{
    public string ActivitySourceName => "AssociationRegistry";
    public string MeterName => "ScheduledHost";

    private readonly Meter _meter;

    public ScheduledHostInstrumentation()
    {
        var version = typeof(ScheduledHostInstrumentation).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        _meter = new Meter(MeterName, version);
    }

    public ActivitySource ActivitySource { get; }

    public void Dispose()
    {
        ActivitySource.Dispose();
        _meter.Dispose();
    }
}
