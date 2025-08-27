namespace AssociationRegistry.Integrations.Slack;

using global::Slack.Webhooks;
using Microsoft.Extensions.Logging;

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

    public async Task Notify(INotification notification)
    {
        try
        {
            var postAsync = await _slackClient.PostAsync(new SlackMessage
            {
                Channel = string.Empty,
                Markdown = true,
                Text = notification.Value,
                IconEmoji = notification.Type switch
                {
                    NotifyType.None => Emoji.Bulb,
                    NotifyType.Success => Emoji.Up,
                    NotifyType.Failure => Emoji.X,
                },
                Username = "Adres sync",
            });

            if(!postAsync)
            {
                _logger.LogWarning($"Slack bericht kon niet verstuurd worden: '{notification.Value}' ({notification.Type})");
            }
            else
            {
                _logger.LogInformation($"Slack bericht verstuurd: '{notification.Value}' ({notification.Type})");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Slack bericht kon niet verstuurd worden");
            return;
        }
    }
}

public record SlackWebhook(string Url);
