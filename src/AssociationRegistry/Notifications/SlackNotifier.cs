namespace AssociationRegistry.Admin.Api.Notifications;

using AssociationRegistry.Notifications;
using Slack.Webhooks;

public class SlackNotifier : INotifier
{
    private readonly ILogger<SlackNotifier> _logger;
    private SlackClient _slackClient;

    public SlackNotifier(SlackWebhook webhookUrl, ILogger<SlackNotifier> logger)
    {
        if (webhookUrl.Url == null) throw new ArgumentNullException(nameof(webhookUrl));
        _logger = logger;

        _slackClient = new SlackClient(webhookUrl.Url);
    }

    public async Task Notify(IMessage message)
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
                NotifyType.Failure => Emoji.X,
            },
            Username = "Adres sync",
        });

        if(!postAsync)
        {
            _logger.LogWarning($"Slack bericht kon niet verstuurd worden: '{message.Value}' ({message.Type})");
        }
        else
        {
            _logger.LogInformation($"Slack bericht verstuurd: '{message.Value}' ({message.Type})");
        }
    }
}

public record SlackWebhook(string Url);
