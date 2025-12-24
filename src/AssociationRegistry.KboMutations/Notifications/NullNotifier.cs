namespace AssociationRegistry.KboMutations.Notifications;

using Integrations.Slack;
using Microsoft.Extensions.Logging;

public class NullNotifier : INotifier
{
    private readonly ILogger<NullNotifier> _logger;

    public NullNotifier(ILogger<NullNotifier> logger)
    {
        _logger = logger;
    }

    public async Task Notify(INotification message) => _logger.LogInformation("Not notifying slack: {Message}", message.Value);
}
