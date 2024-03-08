namespace AssociationRegistry.Notifications;

public interface INotifier
{
    Task Notify(IMessage message);
}
