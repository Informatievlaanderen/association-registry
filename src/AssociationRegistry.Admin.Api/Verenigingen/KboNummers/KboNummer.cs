namespace AssociationRegistry.Admin.Api.Verenigingen.KboNummers;

using Exceptions;

public class KboNummer
{
    private KboNummer(string kboNummer)
    {
        Value = kboNummer;
    }

    public string Value { get; }

    public override string ToString()
        => Value;

    public static KboNummer? Create(string? maybeKboNummer)
    {
        if (maybeKboNummer is not { } kboNummer)
            return null;

        var value = Sanitize(kboNummer);

        Validate(value);

        return new KboNummer(value);
    }

    private static string Sanitize(string kboNummer)
        => kboNummer
            .Replace(" ", string.Empty)
            .Replace(".", string.Empty);

    private static void Validate(string value)
    {
        Throw<InvalidKboNummer>.If(value.Length != 10);
        Throw<InvalidKboNummer>.If(!ulong.TryParse(value, out _));
    }
}
