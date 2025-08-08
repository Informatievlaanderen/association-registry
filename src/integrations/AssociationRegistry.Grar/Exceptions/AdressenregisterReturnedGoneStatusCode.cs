namespace AssociationRegistry.Grar.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Net;

public class AdressenregisterReturnedGoneStatusCode(string message) : DomainException(message)
{
    public HttpStatusCode StatusCode = HttpStatusCode.Gone;

    public AdressenregisterReturnedGoneStatusCode() : this(ExceptionMessages.AdresVerwijderd)
    {
    }
}
