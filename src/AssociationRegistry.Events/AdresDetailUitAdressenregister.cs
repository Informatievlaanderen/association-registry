namespace AssociationRegistry.Events;

public record AdresDetailUitAdressenregister
{
    public Registratiedata.AdresId AdresId { get; init; }
    public Registratiedata.AdresUitAdressenregister Adres { get; init; }
}
