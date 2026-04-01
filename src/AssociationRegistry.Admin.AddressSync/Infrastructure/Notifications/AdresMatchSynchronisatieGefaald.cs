namespace AssociationRegistry.Admin.AddressSync.Infrastructure.Notifications;

using Integrations.Slack;

public class AdresMatchSynchronisatieGefaald(AdressSyncError[] errors) : INotification
{
    public string Value
    {
        get
        {
            var details = string.Join(
                Environment.NewLine,
                errors.Select(e =>
                                   $"Vcode: {e.VCode}, LocatieIds: [{string.Join(", ", e.LocatieIds)}]"));
            return $"Het synchroniseren van de locaties met het adressenregister is gefaald!{Environment.NewLine}{details}";
        }
    }   public NotifyType Type => NotifyType.Failure;
}
