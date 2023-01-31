namespace AssociationRegistry.INSZ;

using Exceptions;

public class Insz
{
    public static Insz Create(string insz)
    {
        var maybeInsz = insz.Replace(".", string.Empty).Replace("-", string.Empty);
        if (maybeInsz.Length != 11)
            throw new InvalidInszLength();
        if (!Modulo97Correct(maybeInsz))
            throw new InvalidInszMod97();
        throw new InvalidInszChars();
    }

    private static bool Modulo97Correct(string value)
    {
        var baseNumber = int.Parse(value[..9]);
        var remainder = int.Parse(value[9..]);

        var modulo97 = 97 - baseNumber % 97;
        return modulo97 == remainder;
    }
}
