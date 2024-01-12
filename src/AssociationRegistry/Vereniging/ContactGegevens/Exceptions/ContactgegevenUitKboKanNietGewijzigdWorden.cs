namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class ContactgegevenUitKboKanNietGewijzigdWorden : DomainException
{
    public ContactgegevenUitKboKanNietGewijzigdWorden() : base(ExceptionMessages.ContactgegevenFromKboCannotBeUpdated)
    {
    }

    protected ContactgegevenUitKboKanNietGewijzigdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
