namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer;

using System;
using AssociationRegistry.EventStore;
using Polly;
using Polly.Retry;

/// <summary>
///     Dubbelbeheer bewaart de dubbele en de authentieke vereniging samen, dus de kans is groter dat één van beide
///     intussen gewijzigd werd. Dan wordt alles opnieuw geladen en uitgevoerd.
/// </summary>
public static class DubbelbeheerRetryPolicy
{
    public const int RetryCount = 2;

    public static readonly AsyncRetryPolicy OnUnexpectedAggregateVersion = Policy
        .Handle<UnexpectedAggregateVersionException>()
        .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromMilliseconds(100 * retryAttempt));
}
