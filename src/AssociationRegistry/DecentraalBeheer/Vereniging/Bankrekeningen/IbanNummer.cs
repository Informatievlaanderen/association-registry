namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

using Exceptions;
using IbanBic;

public record IbanNummer
{
    private const string BelgianIbanPrefix = "BE";
    public string Value { get; }

    private IbanNummer(string value)
    {
        Value = value;
    }

    public static IbanNummer Create(string iban)
    {
        if (!IsValid(iban))
            throw new IbanFormaatIsOngeldig();

        var sanitized = Sanitize(iban);
        var formatted = FormatWithSpaces(sanitized);

        return new IbanNummer(formatted);
    }

    private static string FormatWithSpaces(string iban)
    {
        return string.Join(" ", Enumerable.Range(0, iban.Length / 4).Select(i => iban.Substring(i * 4, 4)));
    }

    public static IbanNummer Hydrate(string iban) => new(iban);

    public override string ToString() => Value;

    private static string Sanitize(string insz) =>
        insz.Replace(oldValue: ".", string.Empty).Replace(oldValue: " ", string.Empty);

    public static bool IsValid(string iban) => HasBelgianPrefix(iban) && IbanUtils.IsValid(Sanitize(iban), out _);

    private static bool HasBelgianPrefix(string iban) => iban.StartsWith(BelgianIbanPrefix);
}
