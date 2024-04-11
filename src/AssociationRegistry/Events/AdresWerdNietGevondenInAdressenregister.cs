namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record AdresWerdNietGevondenInAdressenregister(string VCode, int LocatieId,  string Straatnaam, string Huisnummer, string Busnummer, string Postcode, string Gemeente) : IEvent
{
}
