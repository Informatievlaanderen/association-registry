namespace AssociationRegistry.Vereniging;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Events;
using Exceptions;
using Framework;
using Kbo;

public record Adres
{
    public const string België = "België";

    private Adres(string straatnaam, string huisnummer, string busnummer, string postcode, string gemeente, string land)
    {
        Straatnaam = straatnaam;
        Huisnummer = huisnummer;
        Busnummer = busnummer;
        Postcode = postcode;
        Gemeente = Gemeentenaam.FromValue(gemeente);
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

    public string Straatnaam { get; init; }
    public string Huisnummer { get; init; }
    public string Busnummer { get; init; }
    public string Postcode { get; set; }
    public Gemeentenaam Gemeente { get; init; }
    public string Land { get; init; }

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

    public string ToAdresString()
        => $"{Straatnaam} {Huisnummer}" +
           (!string.IsNullOrWhiteSpace(Busnummer) ? $" bus {Busnummer}" : string.Empty) +
           $", {Postcode} {Gemeente}, {Land}";

    public static Adres Hydrate(Registratiedata.AdresUitAdressenregister adres)
        => Create(adres.Straatnaam,
                  adres.Huisnummer,
                  adres.Busnummer,
                  adres.Postcode,
                  adres.Gemeente,
                  België);
}

public record Gemeentenaam(string Naam)
{
    public static Gemeentenaam FromValue(string gemeente)
        => new(gemeente);

    public static Gemeentenaam FromVerrijkteGemeentenaam(VerrijkteGemeentenaam gemeentenaam)
        => new(gemeentenaam.Format());
}
