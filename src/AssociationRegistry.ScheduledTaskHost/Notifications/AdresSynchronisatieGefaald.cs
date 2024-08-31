namespace AssociationRegistry.ScheduledTaskHost.Notifications;

using AssociationRegistry.Notifications;

public class AdresSynchronisatieGefaald(Exception exception) : IMessage
{
    public string Value => $"Het synchroniseren van de adressen met adressenregister is gefaald! {exception.Message}";
    public NotifyType Type => NotifyType.Failure;
}
