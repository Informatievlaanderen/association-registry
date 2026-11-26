namespace AssociationRegistry.Scheduled.Host.Infrastructure.Notifications;

using AssociationRegistry.Integrations.Slack;

public class VervalRelatieaanvragenGefaald(Exception exception) : INotification
{
    public string Value => $"Het automatisch verval van de relatieaanvragen is gefaald! {exception.Message}";
    public NotifyType Type => NotifyType.Failure;
}
