namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Search.Exceptions;

using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class ZoekOpdrachtWasIncorrect : DomainException
{
    public ZoekOpdrachtWasIncorrect() : base(ExceptionMessages.ZoekOpdrachtWasIncorrect)
    {
    }
}
