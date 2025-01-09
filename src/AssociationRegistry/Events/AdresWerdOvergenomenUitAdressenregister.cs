namespace AssociationRegistry.Events;



public record AdresWerdOvergenomenUitAdressenregister(
    string VCode,
    int LocatieId,
    Registratiedata.AdresId AdresId,
    Registratiedata.AdresUitAdressenregister Adres) : IEvent;
