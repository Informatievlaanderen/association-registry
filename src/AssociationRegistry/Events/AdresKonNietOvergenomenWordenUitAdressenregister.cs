namespace AssociationRegistry.Events;

using Framework;

public record AdresKonNietOvergenomenWordenUitAdressenregister(string VCode, int LocatieId, string Adres, string Reden = "") : IEvent
{
    public const string RedenLocatieWerdVerwijderd = "Locatie kon niet gevonden worden. Mogelijks is deze verwijderd.";
}
