namespace AssociationRegistry.KboMutations.SyncLambda.Telemetry;

using Messaging;
using System.Diagnostics;

public class SyncEnvelopeActivity : IDisposable
{
    private readonly Activity? _activity;

    private SyncEnvelopeActivity(Activity? activity)
    {
        _activity = activity;
    }

    public static SyncEnvelopeActivity Start(SyncEnvelope envelope)
    {
        var activity = CreateActivity(envelope.ParentTraceContext);

        // Note: SourceFileName is NOT tagged - it's high cardinality (unique per mutation file)
        // and would explode the number of unique time series in the telemetry backend

        return new SyncEnvelopeActivity(activity);
    }

    public void TagAsKboSync(string kboNummer)
    {
        // Note: KBO nummer is NOT tagged - it's high cardinality and would explode the number of unique time series
        _activity?.SetTag(SemanticConventions.MessageType, SemanticConventions.MessageTypes.SyncKbo);
    }

    public void TagAsKszSync()
    {
        _activity?.SetTag(SemanticConventions.MessageType, SemanticConventions.MessageTypes.SyncKsz);
    }

    private static Activity? CreateActivity(ActivityContext? parentContext)
    {
        return parentContext.HasValue
            ? KboSyncActivitySource.Source.StartActivity(
                "ProcessSyncMessage",
                ActivityKind.Consumer,
                parentContext.Value)
            : KboSyncActivitySource.Source.StartActivity(
                "ProcessSyncMessage",
                ActivityKind.Consumer);
    }

    public void Dispose()
    {
        _activity?.Dispose();
    }
}
