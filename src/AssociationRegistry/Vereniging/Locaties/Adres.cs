namespace AssociationRegistry.Vereniging;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Events;
using Exceptions;
using Framework;
using Kbo;
using System.Text;
using System.Text.RegularExpressions;

public record Adres
{
    public const string België = "België";

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

    public string Straatnaam { get; init; }
    public string Huisnummer { get; init; }
    public string Busnummer { get; init; }
    public string Postcode { get; set; }
    public string Gemeente { get; init; }
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

    public static bool AreDuplicates(Adres? adres, Adres? otherAdres)
    {
        if (adres is null && otherAdres is null) return false;
        if (adres is null && otherAdres is not null) return false;
        if (adres is not null && otherAdres is null) return false;

        return NormalizeString(adres!.Straatnaam) == NormalizeString(otherAdres!.Straatnaam) &&
               NormalizeString(adres.Postcode) == NormalizeString(otherAdres.Postcode) &&
               NormalizeString(adres.Huisnummer) == NormalizeString(otherAdres.Huisnummer) &&
               NormalizeString(adres.Busnummer) == NormalizeString(otherAdres.Busnummer) &&
               NormalizeString(adres.Gemeente) == NormalizeString(otherAdres.Gemeente) &&
               NormalizeString(adres.Land) == NormalizeString(otherAdres.Land);
    }

    private static string NormalizeString(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Convert to lowercase
        input = input.ToLowerInvariant();

        // Normalize to decompose accented characters
        input = input.Normalize(NormalizationForm.FormD);

        // Remove diacritics
        input = Regex.Replace(input, @"\p{Mn}", "");

        // Remove all non-alphanumeric characters
        input = Regex.Replace(input, "[^a-z0-9]", string.Empty);

        return input;
    }
}
