namespace AssociationRegistry.KboMutations.Notifications;

using Amazon.Lambda.Core;
using AssociationRegistry.Notifications;

public class NullNotifier : INotifier
{
    private readonly ILambdaLogger _logger;

    public NullNotifier(ILambdaLogger logger)
    {
        _logger = logger;
    }

    public async Task Notify(IMessage message) => _logger.LogInformation($"Not notifying slack: {message.Value}");
}