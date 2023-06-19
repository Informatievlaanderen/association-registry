namespace AssociationRegistry.Vereniging;

public record Locatie
{
    public int LocatieId { get; set; }
    public string? Naam { get; }
    public string Straatnaam { get; }
    public string Huisnummer { get; }
    public string? Busnummer { get; }
    public string Postcode { get; set; }
    public string Gemeente { get; }
    public string Land { get; }
    public bool Hoofdlocatie { get; }
    public string Locatietype { get; }

    private Locatie(string? naam, string straatnaam, string huisnummer, string? busnummer, string postcode, string gemeente, string land, bool hoofdlocatie, string locatietype)
    {
        Naam = naam;
        Straatnaam = straatnaam;
        Huisnummer = huisnummer;
        Busnummer = busnummer;
        Postcode = postcode;
        Gemeente = gemeente;
        Land = land;
        Hoofdlocatie = hoofdlocatie;
        Locatietype = locatietype;
    }

    public static Locatie Create(string? naam, string straatnaam, string huisnummer, string? busnummer, string postcode, string gemeente, string land, bool hoofdlocatie, string locatieType)
        => new(naam, straatnaam, huisnummer, busnummer, postcode, gemeente, land, hoofdlocatie, locatieType);

    public static Locatie Hydrate(int locatieId, string? naam, string straatnaam, string huisnummer, string? busnummer, string postcode, string gemeente, string land, bool hoofdlocatie, string locatieType)
        => new(naam, straatnaam, huisnummer, busnummer, postcode, gemeente, land, hoofdlocatie, locatieType) { LocatieId = locatieId };
}
