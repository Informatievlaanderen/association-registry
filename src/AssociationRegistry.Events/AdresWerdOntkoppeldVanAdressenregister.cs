namespace AssociationRegistry.Events;



public record AdresWerdOntkoppeldVanAdressenregister(string VCode, int LocatieId, Registratiedata.AdresId? AdresId, Registratiedata.Adres? Adres) : IEvent;

