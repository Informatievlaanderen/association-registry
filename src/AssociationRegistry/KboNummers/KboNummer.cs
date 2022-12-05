namespace AssociationRegistry.KboNummers;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;

public class KboNummer : StringValueObject<KboNummer>
{
    private KboNummer(string kboNummer) : base(kboNummer)
    {
    }

    public override string ToString()
        => Value;

    public static KboNummer? Create(string? maybeKboNummer)
    {
        if (string.IsNullOrEmpty(maybeKboNummer))
            return null;

        var value = Sanitize(maybeKboNummer!);

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
