namespace AssociationRegistry.KboMutations.SyncLambda.Telemetry;

using Messaging;
using System.Diagnostics;

public class SyncMessageActivity : IDisposable
{
    private readonly Activity? _activity;

    private SyncMessageActivity(Activity? activity)
    {
        _activity = activity;
    }

    public static SyncMessageActivity Start(MagdaEnvelope envelope)
    {
        var activity = CreateActivity(envelope.ParentTraceContext);

        if (activity != null && !string.IsNullOrEmpty(envelope.SourceFileName))
        {
            activity.SetTag("source.file.name", envelope.SourceFileName);
        }

        return new SyncMessageActivity(activity);
    }

    public void TagAsKboSync(string kboNummer)
    {
        _activity?.SetTag("kbo.nummer", kboNummer);
        _activity?.SetTag("message.type", "SyncKbo");
    }

    public void TagAsKszSync(string insz, bool overleden)
    {
        _activity?.SetTag("insz", insz);
        _activity?.SetTag("overleden", overleden);
        _activity?.SetTag("message.type", "SyncKsz");
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
