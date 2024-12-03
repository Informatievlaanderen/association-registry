namespace AssociationRegistry.Grar.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Net;

public class AdressenregisterReturnedClientErrorStatusCode : DomainException
{
    public HttpStatusCode StatusCode { get; }

    public AdressenregisterReturnedClientErrorStatusCode(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
