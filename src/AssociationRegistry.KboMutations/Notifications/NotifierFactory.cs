namespace AssociationRegistry.KboMutations.Notifications;

using Configuration;
using Integrations.Slack;
using Microsoft.Extensions.Logging;

public class NotifierFactory
{
    private readonly SsmClientWrapper _ssmClientWrapper;
    private readonly ISlackConfiguration _paramNames;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<NotifierFactory> _logger;

    public NotifierFactory(
        SsmClientWrapper ssmClientWrapper,
        ISlackConfiguration paramNames,
        ILoggerFactory loggerFactory)
    {
        _ssmClientWrapper = ssmClientWrapper;
        _paramNames = paramNames;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<NotifierFactory>();
    }

    public async Task<INotifier> Create()
    {
        if (string.IsNullOrEmpty(_paramNames.SlackWebhook))
        {
            _logger.LogWarning("ParamName '{ParamName}' was not provided, slack notifications will not be enabled", nameof(_paramNames.SlackWebhook));

            return new NullNotifier(_loggerFactory.CreateLogger<NullNotifier>());
        }

        var webhook = await _ssmClientWrapper.GetParameterAsync(_paramNames.SlackWebhook);

        LogIfNotFound(webhook, _paramNames.SlackWebhook);

        _logger.LogInformation("Slack notifications are enabled");

        return new SlackNotifier(_loggerFactory.CreateLogger<SlackNotifier>(), webhook);
    }

    public async Task<INotifier> TryCreate()
    {
        try
        {
            return await Create();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Could not create notifier: {ErrorMessage}", ex.Message);
            return new NullNotifier(_loggerFactory.CreateLogger<NullNotifier>());
        }
    }

    private void LogIfNotFound(string value, string parameterName)
    {
        if(string.IsNullOrEmpty(value))
            _logger.LogWarning("Could not fetch '{ParameterName}' value from SSM, slack notifications will not be enabled", parameterName);
    }
}
