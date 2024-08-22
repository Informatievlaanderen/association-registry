namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class LaatsteHoofdActiviteitKanNietVerwijderdWorden : DomainException
{
    public LaatsteHoofdActiviteitKanNietVerwijderdWorden() : base(ExceptionMessages.LaatsteHoofdActiviteitKanNietVerwijderdWorden)
    {
    }

    protected LaatsteHoofdActiviteitKanNietVerwijderdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
