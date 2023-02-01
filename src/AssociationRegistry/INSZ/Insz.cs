namespace AssociationRegistry.INSZ;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;

public class Insz : StringValueObject<Insz>
{
    private Insz(string @string) : base(@string)
    {
    }

    public static Insz Create(string insz)
    {
        var sanitezedInsz = Sanitize(insz);
        Validate(sanitezedInsz);
        return new Insz(sanitezedInsz);
    }

    public override string ToString()
        => Value;

    private static void Validate(string sanitezedInsz)
    {
        Throw<InvalidInszLength>.If(sanitezedInsz.Length != 11);
        Throw<InvalidInszChars>.IfNot(ulong.TryParse(sanitezedInsz, out _));
        Throw<InvalidInszMod97>.IfNot(Modulo97Correct(sanitezedInsz) ^ Modulo97Correct($"2{sanitezedInsz}"));
    }

    private static string Sanitize(string insz)
        => insz.Replace(".", string.Empty).Replace("-", string.Empty);

    private static bool Modulo97Correct(string value)
    {
        var baseNumber = long.Parse(value[..^2]);
        var remainder = long.Parse(value[^2..]);

        var modulo97 = 97 - baseNumber % 97;
        return modulo97 == remainder;
    }
}
