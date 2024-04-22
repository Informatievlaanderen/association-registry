namespace AssociationRegistry.Grar.Exceptions;

using System.Net;

public class AdressenregisterReturnedNonSuccessStatusCode : ApplicationException
{
    public HttpStatusCode StatusCode { get; }

    public AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode statusCode) : this(statusCode, ExceptionMessages.AdresKonNietOvergenomenWorden)
    {
    }

    public AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
