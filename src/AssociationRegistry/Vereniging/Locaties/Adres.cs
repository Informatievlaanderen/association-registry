namespace AssociationRegistry.Vereniging;

public record Adres
{
    public Adres(string straatnaam, string huisnummer, string busnummer, string postcode, string gemeente, string land)
    {
        Straatnaam = straatnaam;
        Huisnummer = huisnummer;
        Busnummer = busnummer;
        Postcode = postcode;
        Gemeente = gemeente;
        Land = land;
    }

    public string Straatnaam { get; }
    public string Huisnummer { get; }
    public string? Busnummer { get; }
    public string Postcode { get; set; }
    public string Gemeente { get; }
    public string Land { get; }

    public AdresId AdresId { get; }
}

public record AdresId()
{
    public static AdresId Create(Adresbron Adresbron, string Waarde)
        => new();
}
