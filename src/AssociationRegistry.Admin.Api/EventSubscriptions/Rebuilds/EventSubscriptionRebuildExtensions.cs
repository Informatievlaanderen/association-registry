namespace AssociationRegistry.Admin.Api.EventSubscriptions.Rebuilds;

using Marten.Events.Daemon.Coordination;

public static class EventSubscriptionRebuildExtensions
{
    public static async Task StartRebuildSubscription(string shardName, IProjectionCoordinator coordinator, long sequence)
    {
        var daemon = coordinator.DaemonForMainDatabase();

        await daemon.RewindSubscriptionAsync(shardName,
                                             CancellationToken.None,
                                             sequence);
    }
}
