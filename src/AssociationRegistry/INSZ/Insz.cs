namespace AssociationRegistry.INSZ;

using Exceptions;

public class Insz
{
    public static Insz Create(string insz)
    {
        throw new InvalidInszChars();
    }
}
