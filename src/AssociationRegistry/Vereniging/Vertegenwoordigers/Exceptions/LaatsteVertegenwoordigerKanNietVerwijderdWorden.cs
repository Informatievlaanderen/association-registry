namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class LaatsteVertegenwoordigerKanNietVerwijderdWorden : DomainException
{
    public LaatsteVertegenwoordigerKanNietVerwijderdWorden() : base(ExceptionMessages.LaatsteVertegenwoordigerKanNietVerwijderdWorden)
    {
    }

    protected LaatsteVertegenwoordigerKanNietVerwijderdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
