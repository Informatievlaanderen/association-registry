namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

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
