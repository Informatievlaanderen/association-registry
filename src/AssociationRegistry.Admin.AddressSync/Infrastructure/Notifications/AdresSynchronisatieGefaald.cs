namespace AssociationRegistry.Admin.AddressSync.Infrastructure.Notifications;

using AssociationRegistry.Notifications;

public class AdresOphalenUitAdressenregisterGefaald : IMessage
{
    private readonly Exception _exception;
    private readonly string _adresId;

    public AdresOphalenUitAdressenregisterGefaald(Exception exception, string adresId)
    {
        _exception = exception;
        _adresId = adresId;
    }

    public string Value => $"Het adres met id: {_adresId} kon niet opgehaald worden uit het adressenregister! {_exception.Message}";
    public NotifyType Type => NotifyType.Failure;
}
