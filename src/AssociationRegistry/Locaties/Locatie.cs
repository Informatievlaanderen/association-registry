namespace AssociationRegistry.Locaties;

public class Locatie
{
    public string? Naam { get; }
    public string Straatnaam { get; }
    public string Huisnummer { get; }
    public string? Busnummer { get; }
    public string Postcode { get; }
    public string Gemeente { get; }
    public string Land { get; }
    public bool IsHoofdlocatie { get; }
    public string Locatietype { get; }

    private Locatie(string? naam, string straatnaam, string huisnummer, string? busnummer, string postcode, string gemeente, string land, bool isHoofdlocatie, string locatietype)
    {
        Naam = naam;
        Straatnaam = straatnaam;
        Huisnummer = huisnummer;
        Busnummer = busnummer;
        Postcode = postcode;
        Gemeente = gemeente;
        Land = land;
        IsHoofdlocatie = isHoofdlocatie;
        Locatietype = locatietype;
    }

    public static Locatie CreateInstance(string? naam, string straatnaam, string huisnummer, string? busnummer, string postcode, string gemeente, string land, bool isHoofdlocatie, string locatieType)
        => new(naam, straatnaam, huisnummer, busnummer, postcode, gemeente, land, isHoofdlocatie, locatieType);
}
