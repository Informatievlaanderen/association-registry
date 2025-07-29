namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using JasperFx.Events;
using Marten;
using Marten.Events.Projections;
using Nest;
using Polly;
using Polly.Retry;
using Policy = Polly.Policy;

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
                                                    TimeSpan.FromSeconds(Math.Min(Math.Pow(x: 2, retryAttempt), maxDelay.TotalSeconds)));
    }

    public void Apply(
        IDocumentOperations operations,
        IReadOnlyList<StreamAction> streams
    )
    {
        throw new NotSupportedException("Subscription should be only run asynchronously");
    }

    public async Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<IEvent> events, CancellationToken cancellation)
        => await _retryPolicy.ExecuteAsync(() => _consumer.ConsumeAsync(events));

}
