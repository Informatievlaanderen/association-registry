namespace AssociationRegistry.Scheduled.Host.Infrastructure.Notifications;

using AssociationRegistry.Integrations.Slack;

public class IngestDubbelBeheerGefaald(Exception exception) : INotification
{
    public string Value =>
        $"Het inlezen van de dubbele verenigingen is gefaald! {exception.GetBaseException().Message}";
    public NotifyType Type => NotifyType.Failure;
}
