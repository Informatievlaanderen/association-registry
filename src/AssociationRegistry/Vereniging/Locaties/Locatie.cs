namespace AssociationRegistry.Vereniging;

public record Locatie
{
    public int LocatieId { get; set; }
    public string? Naam { get; }
    public bool Hoofdlocatie { get; }
    public string Locatietype { get; }
    public Adres Adres { get; }

    private Locatie(string? naam, string straatnaam, string huisnummer, string? busnummer, string postcode, string gemeente, string land, bool hoofdlocatie, string locatietype)
    {
        Naam = naam;
        Adres = new Adres(straatnaam, huisnummer, busnummer, postcode, gemeente, land);
        Hoofdlocatie = hoofdlocatie;
        Locatietype = locatietype;
    }

    public static Locatie Create(string? naam, string straatnaam, string huisnummer, string? busnummer, string postcode, string gemeente, string land, bool hoofdlocatie, string locatieType)
        => new(naam, straatnaam, huisnummer, busnummer, postcode, gemeente, land, hoofdlocatie, locatieType);

    public static Locatie Hydrate(int locatieId, string? naam, string straatnaam, string huisnummer, string? busnummer, string postcode, string gemeente, string land, bool hoofdlocatie, string locatieType)
        => new(naam, straatnaam, huisnummer, busnummer, postcode, gemeente, land, hoofdlocatie, locatieType) { LocatieId = locatieId };
}
