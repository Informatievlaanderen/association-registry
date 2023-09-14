namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
