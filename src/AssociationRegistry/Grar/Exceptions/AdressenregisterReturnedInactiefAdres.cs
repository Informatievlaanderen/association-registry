namespace AssociationRegistry.Grar.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using AssociationRegistry.Resources;
using System.Net;

public class AdressenregisterReturnedInactiefAdres : DomainException
{
    public HttpStatusCode StatusCode { get; }

    public AdressenregisterReturnedInactiefAdres() : base(ExceptionMessages.AdresInactief)
    {
    }
}
