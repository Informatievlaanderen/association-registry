namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class LaatsteLocatieKanNietVerwijderdWorden : DomainException
{
    public LaatsteLocatieKanNietVerwijderdWorden() : base(ExceptionMessages.LaatsteLocatieKanNietVerwijderdWorden)
    {
    }

    protected LaatsteLocatieKanNietVerwijderdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
