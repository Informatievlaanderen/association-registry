namespace AssociationRegistry.KboMutations.Notifications;

using Amazon.Lambda.Core;
using Configuration;
using Integrations.Slack;

public class NotifierFactory
{
    private readonly SsmClientWrapper _ssmClientWrapper;
    private readonly ISlackConfiguration _paramNames;
    private readonly ILambdaLogger _logger;

    public NotifierFactory(SsmClientWrapper ssmClientWrapper,
        ISlackConfiguration paramNames,
        ILambdaLogger logger)
    {
        _ssmClientWrapper = ssmClientWrapper;
        _paramNames = paramNames;
        _logger = logger;
    }

    public async Task<INotifier> Create()
    {
        if (string.IsNullOrEmpty(_paramNames.SlackWebhook))
        {
            _logger.LogWarning($"ParamName '{nameof(_paramNames.SlackWebhook)}' was not provided, slack notifications will not be enabled");

            return new NullNotifier(_logger);
        }

        var webhook = await _ssmClientWrapper.GetParameterAsync(_paramNames.SlackWebhook);

        LogIfNotFound(webhook, _paramNames.SlackWebhook);

        _logger.LogInformation("Slack notifications are enabled");

        return new SlackNotifier(_logger, webhook);
    }

    public async Task<INotifier> TryCreate()
    {
        try
        {
            return await Create();
        }
        catch(Exception ex)
        {
            _logger.LogError($"Could not create notifier: {ex.Message}");
            return new NullNotifier(_logger);
        }
    }

    private void LogIfNotFound(string value, string parameterName)
    {
        if(string.IsNullOrEmpty(value))
            _logger.LogWarning($"Could not fetch '{parameterName}' value from SSM, slack notifications will not be enabled");
    }
}
