namespace AssociationRegistry.Test.MagdaSync.KboSync.ResilienceTests
{
    using KboMutations.SyncLambda.Resilience;
    using Microsoft.Extensions.Logging.Abstractions;
    using Polly.Utilities;
    using Xunit;

    public class RetryPolicyTests
    {
        [Fact]
        public async Task CreateRetryPolicy_WhenActionAlwaysThrows_RetriesExactly5Times()
        {
            // Arrange
            var logger = NullLogger.Instance;
            var policy = RetryPolicy.Create(logger);

            var executions = 0;

            var originalSleepAsync = SystemClock.SleepAsync;
            SystemClock.SleepAsync = (_, __) => Task.CompletedTask;

            try
            {
                // Act
                await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                {
                    await policy.ExecuteAsync(async () =>
                    {
                        executions++;
                        await Task.Yield();
                        throw new InvalidOperationException("boom");
                    });
                });

                // Assert
                // retryCount = 5 => total = 1 initial + 5 retries = 6
                Assert.Equal(6, executions);
            }
            finally
            {
                SystemClock.SleepAsync = originalSleepAsync;
            }
        }

        [Fact]
        public async Task CreateRetryPolicy_WhenActionFailsTwiceThenSucceeds_Executes3Times()
        {
            // Arrange
            var logger = NullLogger.Instance;
            var policy = RetryPolicy.Create(logger);

            var executions = 0;

            var originalSleepAsync = SystemClock.SleepAsync;
            SystemClock.SleepAsync = (_, __) => Task.CompletedTask;

            try
            {
                // Act
                await policy.ExecuteAsync(async () =>
                {
                    executions++;
                    await Task.Yield();

                    if (executions <= 2)
                        throw new Exception("transient");
                });

                // Assert
                Assert.Equal(3, executions);
            }
            finally
            {
                SystemClock.SleepAsync = originalSleepAsync;
            }
        }

        [Fact]
        public async Task CreateRetryPolicy_WhenAlwaysThrows_UsesExpectedExponentialBackoffDelays()
        {
            // Arrange
            var logger = NullLogger.Instance;

            // We'll capture the delays Polly *wants* to sleep for
            var observedSleeps = new List<TimeSpan>();

            var originalSleepAsync = SystemClock.SleepAsync;
            SystemClock.SleepAsync = (timespan, _) =>
            {
                observedSleeps.Add(timespan);
                return Task.CompletedTask;
            };

            try
            {
                var policy = RetryPolicy.Create(logger);

                // Act (force all retries)
                await Assert.ThrowsAsync<Exception>(async () =>
                {
                    await policy.ExecuteAsync(() => throw new Exception("boom"));
                });

                // Assert
                // baseDelay = 3s, exponential: 3,6,12,24,48 (5 retries)
                var expected = new[]
                {
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(6),
                    TimeSpan.FromSeconds(12),
                    TimeSpan.FromSeconds(24),
                    TimeSpan.FromSeconds(48),
                };

                Assert.Equal(expected.Length, observedSleeps.Count);
                for (var i = 0; i < expected.Length; i++)
                {
                    Assert.Equal(expected[i], observedSleeps[i]);
                }
            }
            finally
            {
                SystemClock.SleepAsync = originalSleepAsync;
            }
        }
    }
}
