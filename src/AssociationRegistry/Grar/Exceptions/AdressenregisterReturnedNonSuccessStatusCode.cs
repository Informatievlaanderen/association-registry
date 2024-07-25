namespace AssociationRegistry.Grar.Exceptions;

using System.Net;

public class AdressenregisterReturnedNonSuccessStatusCode : ApplicationException
{
    public HttpStatusCode StatusCode { get; }

    public AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode statusCode) : this(statusCode,
                                                                                          statusCode == HttpStatusCode.BadRequest
                                                                                              ? ExceptionMessages.AdresKonNietOvergenomenWorden
                                                                                              : ExceptionMessages.AdresKonNietOvergenomenWordenBadRequest)
    {
    }

    public AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
