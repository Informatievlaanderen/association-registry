namespace AssociationRegistry.Admin.AddressSync.Infrastructure.Notifications;

using Integrations.Slack;

public class AdresSynchronisatieGefaald(Exception exception) : INotification
{
    public string Value => $"Het synchroniseren van de adressen met adressenregister is gefaald! {exception.Message}";
    public NotifyType Type => NotifyType.Failure;
}
