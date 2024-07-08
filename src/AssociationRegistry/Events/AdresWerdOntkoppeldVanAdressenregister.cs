namespace AssociationRegistry.Events;

using Framework;

public record AdresWerdOntkoppeldVanAdressenregister(string VCode, int LocatieId, Registratiedata.AdresId? AdresId, Registratiedata.Adres? Adres) : IEvent;

