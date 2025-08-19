namespace AssociationRegistry.MartenDb.Subscriptions;

using JasperFx.Events;
using Marten;
using Marten.Events.Projections;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Policy = Polly.Policy;

public class MartenSubscription : IProjection
{
    private readonly IMartenEventsConsumer _consumer;
    private readonly Type[] _handledEventTypes;
    private readonly ILogger<MartenSubscription> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public MartenSubscription(IMartenEventsConsumer consumer, Type[] handledEventTypes, ILogger<MartenSubscription> logger)
    {
        _consumer = consumer;
        _handledEventTypes = handledEventTypes;
        _logger = logger;
        var maxDelay = TimeSpan.FromSeconds(30);

        _retryPolicy = Policy
                      .Handle<Exception>()
                      .WaitAndRetryAsync(
                           retryCount: 5,
                           sleepDurationProvider: retryAttempt =>
                           {
                               return TimeSpan.FromSeconds(Math.Min(Math.Pow(x: 2, retryAttempt), maxDelay.TotalSeconds));
                           },
                           onRetryAsync: (exception, delay) =>
                           {
                               logger.LogError(exception, "Error occurred while consuming events. Retrying in {Delay} seconds.",
                                               delay.TotalSeconds);

                               return Task.CompletedTask;
                           });
    }

    public void Apply(
        IDocumentOperations operations,
        IReadOnlyList<StreamAction> streams
    )
    {
        throw new NotSupportedException("Subscription should be only run asynchronously");
    }

    public async Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<IEvent> events, CancellationToken cancellation)
    {
        _logger.LogInformation("STARTING BATCH PROCESSING FOR {Type} EVENTS: {FirstEvent}-{LastEvent}",
                               _consumer.GetType().Name,
                               events.FirstOrDefault()?.Sequence,
                               events.LastOrDefault()?.Sequence);

        await _retryPolicy.ExecuteAsync(() => _consumer.ConsumeAsync(SubscriptionEventList.From(events, _handledEventTypes)));

        _logger.LogInformation("FINISHED BATCH PROCESSING FOR {Type} EVENTS: {FirstEvent}-{LastEvent}",
                               _consumer.GetType().Name,
                               events.FirstOrDefault()?.Sequence,
                               events.LastOrDefault()?.Sequence);

    }
}
