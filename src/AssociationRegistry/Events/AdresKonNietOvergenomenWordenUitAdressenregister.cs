namespace AssociationRegistry.Events;

using Framework;

public record AdresKonNietOvergenomenWordenUitAdressenregister(string VCode, int LocatieId, string Reden = "") : IEvent;
