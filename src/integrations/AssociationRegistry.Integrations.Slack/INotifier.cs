namespace AssociationRegistry.Integrations.Slack;

public interface INotifier
{
    Task Notify(INotification notification);
}
