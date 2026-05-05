namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start.Notifications;

using Integrations.Slack;

public class EventSubscriptionBewaartermijnFailed : INotification
{
    private readonly string _exception;

    public EventSubscriptionBewaartermijnFailed(string exception)
    {
        _exception = exception;
    }

    public string Value => $"Eventsubscription bewaartermijn failed with message: {_exception}.";
    public NotifyType Type => NotifyType.Failure;
}
