namespace AssociationRegistry.KboMutations.SyncLambda.Resilience;

using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

public class RetryPolicy
{
    public static AsyncRetryPolicy Create(ILogger logger)
    {
        var retryCount = 5;
        var baseDelay = TimeSpan.FromSeconds(3);

        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt =>
                {
                    var exp = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1)) * baseDelay.TotalSeconds;

                    return TimeSpan.FromSeconds(exp.TotalSeconds);
                },
                onRetry: (exception, sleep, attempt, context) =>
                {
                    logger.LogWarning(
                        exception,
                        "Retry {Attempt} after {Delay} due to exception. Context={Context}",
                        attempt,
                        sleep,
                        context?.OperationKey
                    );
                }
            );
    }
}
