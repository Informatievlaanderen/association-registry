namespace AssociationRegistry.Events;


public record NietUniekeAdresMatchUitAdressenregister
{

    public Registratiedata.AdresId? AdresId { get; init; }
    public string Adresvoorstelling { get; init; }
    public double Score { get; init; }
}
