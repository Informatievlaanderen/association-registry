namespace AssociationRegistry.Grar.Exceptions;

public class AdresKonNietOvergenomenWorden : ApplicationException
{
    public AdresKonNietOvergenomenWorden() : base(ExceptionMessages.AdresKonNietGevondenWorden)
    {
    }

    public AdresKonNietOvergenomenWorden(string message) : base(message)
    {
    }
}
