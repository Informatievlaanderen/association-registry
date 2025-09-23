namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

public class OngeldigBevestigingsToken : DomainException
{
    public OngeldigBevestigingsToken() : base(ExceptionMessages.OngeldigBevestigingsToken)
    {
    }
}
