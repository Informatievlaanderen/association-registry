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

    /// <summary>
    /// if kboNummer contains spaces or dots, the 5th and the 9th character are deleted
    /// these are the only allowed positions for spaces or dots
    /// </summary>
    /// <param name="kboNummer"></param>
    /// <returns></returns>
    private static string Sanitize(string kboNummer)
        => kboNummer
            .Replace(".", "")
            .Replace(" ", "");

    private static void Validate(string value)
    {
        Throw<InvalidKboNummerChars>.IfNot(ulong.TryParse(value, out _));
        Throw<InvalidKboNummerLength>.If(value.Length != 10);
        Throw<InvalidKboNummerMod97>.IfNot(Modulo97Correct(value));
    }

    private static bool Modulo97Correct(string value)
    {
        var baseNumber = int.Parse(value[..8]);
        var remainder = int.Parse(value[8..]);

        var modulo97 = 97 - baseNumber % 97;
        return modulo97 == remainder;
    }
}
