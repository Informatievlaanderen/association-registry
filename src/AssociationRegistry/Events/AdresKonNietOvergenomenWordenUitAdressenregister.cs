namespace AssociationRegistry.Events;

public record AdresKonNietOvergenomenWordenUitAdressenregister(
    string VCode,
    int LocatieId,
    string Adres,
    string Reden = ""
) : IEvent
{
    public const string RedenLocatieWerdVerwijderd = "Locatie kon niet gevonden worden. Mogelijks is deze verwijderd.";
    public const string RedenAdresKonNietGevalideerdWordenBijAdressenregister =
        "Adres kon niet gevalideerd worden bij adressenregister.";
}
