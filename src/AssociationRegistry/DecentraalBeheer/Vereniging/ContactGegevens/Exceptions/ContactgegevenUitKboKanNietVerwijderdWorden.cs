namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
