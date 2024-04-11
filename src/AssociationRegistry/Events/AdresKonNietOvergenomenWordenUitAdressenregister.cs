namespace AssociationRegistry.Events;

using Framework;

public record AdresKonNietOvergenomenWordenUitAdressenregister(string VCode, int LocatieId, string Adres, string Reden = "") : IEvent;
