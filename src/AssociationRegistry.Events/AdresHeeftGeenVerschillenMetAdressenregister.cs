namespace AssociationRegistry.Events;



public record AdresHeeftGeenVerschillenMetAdressenregister(
    string VCode,
    int LocatieId,
    Registratiedata.AdresId AdresId,
    Registratiedata.AdresUitAdressenregister Adres) : IEvent;
