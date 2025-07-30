namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using JasperFx.Events;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using MartenDb;
using MartenDb.Subscriptions;
using Polly;
using Polly.Retry;

public class MartenSubscription : IProjection
{
    private readonly IMartenEventsConsumer _consumer;
    private readonly AsyncRetryPolicy _retryPolicy;

    public MartenSubscription(IMartenEventsConsumer consumer)
    {
        _consumer = consumer;

        var maxDelay = TimeSpan.FromSeconds(30); // Set the maximum delay limit here
        _retryPolicy = Policy
                      .Handle<Exception>()
                      .WaitAndRetryForeverAsync(retryAttempt =>
                                                    TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt), maxDelay.TotalSeconds)));

    }

    public void Apply(
        IDocumentOperations operations,
        IReadOnlyList<StreamAction> streams
    )
    {
        throw new NotSupportedException("Subscription should be only run asynchronously");
    }

    public async Task ApplyAsync(
        IDocumentOperations operations,
        IReadOnlyList<IEvent> events,
        CancellationToken ct
    )
        => await _retryPolicy.ExecuteAsync(() => _consumer.ConsumeAsync(
                                               SubscriptionEventList.From(events)));
}
