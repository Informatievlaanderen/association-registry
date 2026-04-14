namespace AssociationRegistry.Integrations.Slack;

public interface INotification
{
    public string Value { get; }
    public NotifyType Type { get; }
}
