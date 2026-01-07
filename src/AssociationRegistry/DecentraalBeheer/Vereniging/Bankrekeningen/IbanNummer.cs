namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

using Exceptions;
using IbanBic;

public record IbanNummer
{
    public string Value { get; }

    private IbanNummer(string value)
    {
        Value = value;
    }

    public static IbanNummer Create(string iban)
    {
        var sanitezedIban = Sanitize(iban);
        if(!IbanUtils.IsValid(sanitezedIban, out _))
            throw new IbanFormaatIsOngeldig();

        return new IbanNummer(sanitezedIban);
    }

    public static IbanNummer Hydrate(string iban)
        => new(iban);

    public override string ToString()
        => Value;

    private static string Sanitize(string insz)
        => insz.Replace(oldValue: ".", string.Empty)
               .Replace(oldValue: " ", string.Empty);
}
