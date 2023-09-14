namespace AssociationRegistry.Vereniging;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;
using Kbo;

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
        Throw<AdresIsIncompleet>.If(string.IsNullOrWhiteSpace(straatnaam));
        Throw<AdresIsIncompleet>.If(string.IsNullOrWhiteSpace(huisnummer));
        Throw<AdresIsIncompleet>.If(string.IsNullOrWhiteSpace(postcode));
        Throw<AdresIsIncompleet>.If(string.IsNullOrWhiteSpace(gemeente));
        Throw<AdresIsIncompleet>.If(string.IsNullOrWhiteSpace(land));

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

    public static Adres? TryCreateFromKbo(AdresVolgensKbo adresVolgensKbo)
    {
        try
        {
            return Create(
                adresVolgensKbo.Straatnaam ?? string.Empty,
                adresVolgensKbo.Huisnummer ?? string.Empty,
                adresVolgensKbo.Busnummer,
                adresVolgensKbo.Postcode ?? string.Empty,
                adresVolgensKbo.Gemeente ?? string.Empty,
                adresVolgensKbo.Land ?? string.Empty);
        }
        catch (DomainException)
        {
            return null;
        }
    }
}
