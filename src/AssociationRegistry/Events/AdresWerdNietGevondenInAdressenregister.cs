namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record AdresWerdNietGevondenInAdressenregister(
    string VCode,
    int LocatieId,
    string Straatnaam,
    string Huisnummer,
    string Busnummer,
    string Postcode,
    string Gemeente) : IEvent
{
    public static AdresWerdNietGevondenInAdressenregister From(VCode vCode, Locatie locatie)
        => new(vCode, locatie.LocatieId, locatie.Adres.Straatnaam, locatie.Adres.Huisnummer,
               locatie.Adres.Busnummer, locatie.Adres.Postcode, locatie.Adres.Gemeente.Naam);
}
