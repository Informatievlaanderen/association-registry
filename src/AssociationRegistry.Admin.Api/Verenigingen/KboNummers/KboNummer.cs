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
    {
        // ^ -> XOR
        if (kboNummer.Contains(' ') ^ kboNummer.Contains('.'))
            return kboNummer[..4] + kboNummer[5..8] + kboNummer[9..];

        return kboNummer;
    }

    private static void Validate(string value)
    {
        Throw<InvalidKboNummer>.If(value.Length != 10);
        Throw<InvalidKboNummer>.If(!ulong.TryParse(value, out _));
    }
}
