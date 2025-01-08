namespace AssociationRegistry.Events;



public record AdresWerdNietGevondenInAdressenregister(
    string VCode,
    int LocatieId,
    string Straatnaam,
    string Huisnummer,
    string Busnummer,
    string Postcode,
    string Gemeente) : IEvent
{
}
