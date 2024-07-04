namespace AssociationRegistry.Grar.Exceptions;

using System.Net;

public class AdressenregisterReturnedGoneStatusCode(string message) : ApplicationException(message)
{
    public HttpStatusCode StatusCode = HttpStatusCode.Gone;

    public AdressenregisterReturnedGoneStatusCode() : this(ExceptionMessages.AdresVerwijderd)
    {
    }
}
