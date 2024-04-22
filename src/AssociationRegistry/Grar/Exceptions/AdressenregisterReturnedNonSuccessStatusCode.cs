namespace AssociationRegistry.Grar.Exceptions;

public class AdressenregisterReturnedNonSuccessStatusCode : ApplicationException
{
    public AdressenregisterReturnedNonSuccessStatusCode() : base(ExceptionMessages.AdresKonNietOvergenomenWorden)
    {
    }

    public AdressenregisterReturnedNonSuccessStatusCode(string message) : base(message)
    {
    }
}
