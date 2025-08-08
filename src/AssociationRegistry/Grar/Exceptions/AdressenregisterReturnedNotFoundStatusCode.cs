namespace AssociationRegistry.Grar.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using AssociationRegistry.Resources;
using System.Net;

public class AdressenregisterReturnedNotFoundStatusCode(string message) : DomainException(message)
{
    public HttpStatusCode StatusCode = HttpStatusCode.NotFound;

    public AdressenregisterReturnedNotFoundStatusCode() : this(ExceptionMessages.AdresNietGevonden)
    {
    }
}
