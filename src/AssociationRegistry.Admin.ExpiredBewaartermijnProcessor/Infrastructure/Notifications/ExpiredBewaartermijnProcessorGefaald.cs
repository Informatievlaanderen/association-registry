namespace AssociationRegistry.Admin.ExpiredBewaartermijnProcessor.Infrastructure.Notifications;

using AssociationRegistry.Integrations.Slack;

public class ExpiredBewaartermijnProcessorGefaald(Exception exception) : INotification
{
    public string Value => $"Het verwijderen van de vervallen bewaartermijnen is gefaald! {exception.Message}";
    public NotifyType Type => NotifyType.Failure;
}
