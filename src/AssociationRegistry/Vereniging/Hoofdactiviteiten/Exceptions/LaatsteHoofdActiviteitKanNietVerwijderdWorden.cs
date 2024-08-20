namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class LaatsteHoofdActiviteitKanNietVerwijderdWorden : DomainException
{
    public LaatsteHoofdActiviteitKanNietVerwijderdWorden() : base(ExceptionMessages.VerenigingKanNietVerwijderdWorden)
    {
    }

    protected LaatsteHoofdActiviteitKanNietVerwijderdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
