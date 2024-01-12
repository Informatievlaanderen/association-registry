namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using Marten;
using Marten.Events;
using Marten.Events.Projections;

public class MartenSubscription : IProjection
{
    private readonly IMartenEventsConsumer _consumer;

    public MartenSubscription(IMartenEventsConsumer consumer)
    {
        _consumer = consumer;
    }

    public void Apply(
        IDocumentOperations operations,
        IReadOnlyList<StreamAction> streams
    )
    {
        throw new NotSupportedException("Subscription should be only run asynchronously");
    }

    public Task ApplyAsync(
        IDocumentOperations operations,
        IReadOnlyList<StreamAction> streams,
        CancellationToken ct
    )
        => _consumer.ConsumeAsync(streams);
}
