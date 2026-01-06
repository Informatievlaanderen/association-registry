namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

using Exceptions;
using IbanBic;

public record IBanNummer
{
    public string Value { get; }

    private IBanNummer(string value)
    {
        Value = value;
    }

    public static IBanNummer Create(string iban)
    {
        var sanitezedIban = Sanitize(iban);
        if(!IbanUtils.IsValid(sanitezedIban, out _))
            throw new IbanFormaatIsOngeldig();

        return new IBanNummer(sanitezedIban);
    }

    public static IBanNummer Hydrate(string insz)
        => new(insz);

    public override string ToString()
        => Value;

    private static string Sanitize(string insz)
        => insz.Replace(oldValue: ".", string.Empty)
               .Replace(oldValue: " ", string.Empty);
}
