namespace AssociationRegistry.Grar.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Net;

public class AdressenregisterReturnedNonSuccessStatusCode : DomainException
{
    public HttpStatusCode StatusCode { get; }

    public AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode statusCode) : this(statusCode,
                                                                                          statusCode == HttpStatusCode.BadRequest
                                                                                              ? ExceptionMessages.AdresKonNietGevalideerdWordenBijAdressenregister
                                                                                              : ExceptionMessages.AdresKonNietOvergenomenWorden)
    {
    }

    public AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
