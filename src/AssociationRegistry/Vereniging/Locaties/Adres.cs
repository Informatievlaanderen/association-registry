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
        Straatnaam = new AdresComponent(straatnaam);
        Huisnummer = new AdresComponent(huisnummer);
        Busnummer = new AdresComponent(busnummer);
        Postcode = new AdresComponent(postcode);
        Gemeente = new AdresComponent(gemeente);
        Land = new AdresComponent(land);
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

    public AdresComponent Straatnaam { get; init; }
    public AdresComponent Huisnummer { get; init; }
    public AdresComponent Busnummer { get; init; }
    public AdresComponent Postcode { get; set; }
    public AdresComponent Gemeente { get; init; }
    public AdresComponent Land { get; init; }

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

    public bool IsEquivalentTo(Adres otherAdres)
    {
        return Straatnaam.Equals(otherAdres.Straatnaam) &&
               Postcode.Equals(otherAdres.Postcode) &&
               Huisnummer.Equals(otherAdres.Huisnummer) &&
               Busnummer.Equals(otherAdres.Busnummer) &&
               Gemeente.Equals(otherAdres.Gemeente) &&
               Land.Equals(otherAdres.Land);
    }
}

public class AdresComponent
{
    public AdresComponent(string value)
    {
        _value = value;
    }
    private readonly string _value;
    public bool Equals(AdresComponent? other)
        => NormalizeString(_value) == NormalizeString(other._value);

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != typeof(AdresComponent))
            return false;

        return Equals((AdresComponent)obj);
    }

    public override int GetHashCode()
        => HashCode.Combine(_value);

    private string NormalizeString(string input)
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


    public static implicit operator string(AdresComponent component)
    {
        return component._value;
    }

    public static implicit operator AdresComponent(string value)
    {
        return new AdresComponent(value);
    }
}
