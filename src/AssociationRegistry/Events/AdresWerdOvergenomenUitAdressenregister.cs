namespace AssociationRegistry.Events;

using Framework;

public record AdresWerdOvergenomenUitAdressenregister(
    string VCode,
    int LocatieId,
    Registratiedata.AdresId AdresId,
    Registratiedata.AdresUitAdressenregister Adres) : IEvent;
