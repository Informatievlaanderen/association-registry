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

        var sanitezedIban = Sanitize(iban);

        return new IbanNummer(sanitezedIban);
    }

    public static IbanNummer Hydrate(string iban)
        => new(iban);

    public override string ToString()
        => Value;

    private static string Sanitize(string insz)
        => insz.Replace(oldValue: ".", string.Empty)
               .Replace(oldValue: " ", string.Empty);

    public static bool IsValid(string iban)
        => HasBelgianPrefix(iban)
        && IbanUtils.IsValid(Sanitize(iban), out _);

    private static bool HasBelgianPrefix(string sanitezedIban)
        => sanitezedIban.StartsWith(BelgianIbanPrefix);
}
