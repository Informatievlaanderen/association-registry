namespace AssociationRegistry.Metrics;

using System.Diagnostics.Metrics;

public static class RepositoryMetrics
{
    public const string MeterName    = "VR.EventStore.AggregatesLoaded";
    private const string MeterVersion = "1.0.0";

    private static readonly Meter Meter = new Meter(MeterName, MeterVersion);

    public static readonly Counter<long> AggregatesLoaded = Meter.CreateCounter<long>(
        name:        "aggregate_loaded_total",
        description: "How many times aggregate was loaded"
    );

    public static void RecordAggregateLoaded(bool hasExpectedVersion, string initiator)
    {

        AggregatesLoaded.Add(
            1,
            new KeyValuePair<string, object>("has_expected_version", hasExpectedVersion),
            new KeyValuePair<string, object>("initiator", initiator)
        );
    }
}
