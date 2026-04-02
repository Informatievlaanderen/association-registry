namespace AssociationRegistry.Admin.AddressSync.Infrastructure.Notifications;

using Integrations.Slack;

public class AdresSynchronisatieProcessorGefaald(AdressSyncError[] errors) : INotification
{
    public string Value
    {
        get
        {
            var details = string.Join(
                Environment.NewLine,
                errors.Select(e =>
                                   $"Vcode: {e.VCode}, LocatieIds: [{string.Join(", ", e.LocatieIds)}], exception: {e.Exception.Message}"));

            return $"Het synchroniseren van enkele locaties met het adressenregister is niet gelukt!{Environment.NewLine}{details}";
        }
    }
    public NotifyType Type => NotifyType.Failure;
}
