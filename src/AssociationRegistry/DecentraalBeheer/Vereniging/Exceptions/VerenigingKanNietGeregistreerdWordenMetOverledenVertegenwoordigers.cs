namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingKanNietGeregistreerdWordenMetOverledenVertegenwoordigers: DomainException
{
    public VerenigingKanNietGeregistreerdWordenMetOverledenVertegenwoordigers() : base(ExceptionMessages.VerenigingKanNietGeregistreerdWordenMetOverledenVertegenwoordigers)
    {
    }

    protected VerenigingKanNietGeregistreerdWordenMetOverledenVertegenwoordigers(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
