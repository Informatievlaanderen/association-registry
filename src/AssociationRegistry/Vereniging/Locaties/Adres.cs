namespace AssociationRegistry.Vereniging;

using Exceptions;
using Framework;

public record Adres
{
    private Adres(string straatnaam, string huisnummer, string busnummer, string postcode, string gemeente, string land)
    {
        Straatnaam = straatnaam;
        Huisnummer = huisnummer;
        Busnummer = busnummer;
        Postcode = postcode;
        Gemeente = gemeente;
        Land = land;
    }

    public static Adres Create(string straatnaam, string huisnummer, string? busnummer, string postcode, string gemeente, string land)
    {
        Throw<IncompleteAdres>.If(string.IsNullOrWhiteSpace(straatnaam));
        Throw<IncompleteAdres>.If(string.IsNullOrWhiteSpace(huisnummer));
        Throw<IncompleteAdres>.If(string.IsNullOrWhiteSpace(postcode));
        Throw<IncompleteAdres>.If(string.IsNullOrWhiteSpace(gemeente));
        Throw<IncompleteAdres>.If(string.IsNullOrWhiteSpace(land));

        return new Adres(straatnaam, huisnummer, busnummer ?? string.Empty, postcode, gemeente, land);
    }

    public string Straatnaam { get; }
    public string Huisnummer { get; }
    public string Busnummer { get; }
    public string Postcode { get; set; }
    public string Gemeente { get; }
    public string Land { get; }

    public static Adres Hydrate(string straatnaam, string huisnummer, string busnummer, string postcode, string gemeente, string land)
        => new(straatnaam, huisnummer, busnummer, postcode, gemeente, land);
}
