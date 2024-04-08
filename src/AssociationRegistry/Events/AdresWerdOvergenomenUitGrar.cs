namespace AssociationRegistry.Events;

using Framework;
using Grar.Models;

public record AdresWerdOvergenomenUitGrar(string VCode, int LocatieId, AdresMatchUitGrar OvergenomenAdresUitGrar, AdresMatchUitGrar[] NietOvergenomenAdressenUitGrar) : IEvent;

public record AdresMatchUitGrar(
    string AdresId,
    AdresStatus? AdresStatus,
    double Score,
    string Straatnaam,
    string Huisnummer,
    string Busnummer,
    string Postcode,
    string Gemeentenaam);
