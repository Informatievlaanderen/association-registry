namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class ContactgegevenUitKboKanNietVerwijderdWorden : DomainException
{
    public ContactgegevenUitKboKanNietVerwijderdWorden() : base(ExceptionMessages.ContactgegevenFromKboCannotBeRemoved)
    {
    }

    protected ContactgegevenUitKboKanNietVerwijderdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
