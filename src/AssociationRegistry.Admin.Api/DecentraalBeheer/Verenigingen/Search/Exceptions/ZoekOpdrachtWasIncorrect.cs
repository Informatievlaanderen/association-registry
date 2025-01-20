namespace AssociationRegistry.Admin.Api.Verenigingen.Search.Exceptions;

using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class ZoekOpdrachtWasIncorrect : DomainException
{
    public ZoekOpdrachtWasIncorrect() : base(ExceptionMessages.ZoekOpdrachtWasIncorrect)
    {
    }
}
