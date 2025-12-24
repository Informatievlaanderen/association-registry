namespace AssociationRegistry.KboMutations.Notifications;

using Integrations.Slack;
using Microsoft.Extensions.Logging;
using Slack.Webhooks;

public class SlackNotifier : INotifier
{
    private readonly ILogger<SlackNotifier> _logger;
    private SlackClient _slackClient;

    public SlackNotifier(ILogger<SlackNotifier> logger, string webhookUrl)
    {
        if (webhookUrl == null) throw new ArgumentNullException(nameof(webhookUrl));
        _logger = logger;

        _slackClient = new SlackClient(webhookUrl);
    }

    public async Task Notify(INotification message)
    {
        var postAsync = await _slackClient.PostAsync(new SlackMessage
        {
            Channel = string.Empty,
            Markdown = true,
            Text = message.Value,
            IconEmoji = message.Type switch
            {
                NotifyType.None => Emoji.Bulb,
                NotifyType.Success => Emoji.Up,
                NotifyType.Failure => Emoji.X
            },
            Username = "Kbo Sync"
        });

        if(!postAsync)
        {
            _logger.LogWarning("Slack bericht kon niet verstuurd worden: '{Message}' ({Type})", message.Value, message.Type);
        }
        else
        {
            _logger.LogInformation("Slack bericht verstuurd: '{Message}' ({Type})", message.Value, message.Type);
        }
    }
}
