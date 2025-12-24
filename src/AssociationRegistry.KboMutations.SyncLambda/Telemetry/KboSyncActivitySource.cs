namespace AssociationRegistry.KboMutations.SyncLambda.Telemetry;

using System.Diagnostics;

public static class KboSyncActivitySource
{
    public static readonly ActivitySource Source = new("KboSync", "1.0.0");

    public static Activity? StartSyncKbo(string kboNummer)
    {
        var activity = Source.StartActivity("SyncKbo", ActivityKind.Consumer);
        activity?.SetTag("kbo.nummer", kboNummer);
        return activity;
    }

    public static Activity? StartSyncKsz(string insz, bool overleden)
    {
        var activity = Source.StartActivity("SyncKsz", ActivityKind.Consumer);
        activity?.SetTag("insz", insz);
        activity?.SetTag("overleden", overleden);
        return activity;
    }
}
